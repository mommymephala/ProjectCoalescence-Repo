using System;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TarSpawnAI : MonoBehaviour, IDamageable
{
    public enum State
    {
        InitialSpawn,
        Idling,
        Chasing,
        Attacking,
        Death
    }

    [HideInInspector] public State currentState;
    private NavMeshAgent _agent;
    private Animator _animator;
    private CapsuleCollider _collider;
    private Transform _playerTransform;
    
    private bool _isPlayerDetected = false;
    private bool _isPlayerLastPositionKnown = false;
    private Vector3 _lastKnownPlayerPosition;
    
    public float timeBetweenAttacks = 2f;
    private float _timeSinceLastAttack = 0f;
    
    // Timeout after which the AI will return to idle if the player is not detected
    // public float lostPlayerTimeout = 5f;
    // private float _timeSinceLastSawPlayer = 0f;
    
    private bool _isDead = false;
    [SerializeField] private float health;

    // Parameters for AI behavior
    public float sightRange = 10f;
    public float attackRange = 2f;
    private int _attackCount = 0;
    // private bool _isAttacking = false;
    
    public LayerMask playerLayer;
    // public GameObject lootPrefab;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        // currentState = State.InitialSpawn;
        currentState = State.Idling;
    }

    private void Update()
    {
        if (_isDead) return;
        _timeSinceLastAttack += Time.deltaTime;
        
        if (_playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
                return;
            }
        }

        switch (currentState)
        {
            case State.InitialSpawn:
                // InitialSpawnBehavior();
                break;
            case State.Idling:
                Idle();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
            case State.Death:
                Death();
                break;
            default:
                Debug.Log("The AI is in default state. This shouldn't happen.");
                break;
        }
    }

    // private void InitialSpawnBehavior()
    // {
    //     // Implement your spawn animation logic here
    //     _animator.Play("SpawnAnimation");
    //     currentState = State.Idling;
    // }

    private void Idle()
    {
        Debug.Log("Is in idle state.");
        
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), 0, Time.deltaTime * 2));

        float eyeLevel = transform.position.y + (_agent.height * 0.75f);

        // Parameters for vision cone
        float visionConeAngle = 60f;
        int numberOfRays = 30;
        float angleStep = visionConeAngle / numberOfRays;

        // Detect the player with multiple raycasts
        for (int i = 0; i <= numberOfRays; i++)
        {
            float rayAngle = -visionConeAngle / 2 + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, rayAngle, 0) * transform.forward;

            Vector3 rayOrigin = new Vector3(transform.position.x, eyeLevel, transform.position.z);

            Debug.DrawRay(rayOrigin, direction * sightRange, Color.red);

            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, sightRange, playerLayer))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    _isPlayerDetected = true;
                    currentState = State.Chasing;
                    break;
                }
            }
        }
    }

    private void Chase()
    {
        if (_isPlayerDetected)
        {
            Debug.Log("Is in chase state.");
            
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            
            _agent.SetDestination(_playerTransform.position);
            _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 2));

            if (Vector3.Distance(_playerTransform.position, transform.position) <= attackRange)
            {
                currentState = State.Attacking;
            }
            
            if (Vector3.Distance(_playerTransform.position, transform.position) > sightRange)
            {
                _isPlayerDetected = false;
                _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), 0, Time.deltaTime * 2));
            }
        }
        else if (_isPlayerLastPositionKnown && !_isPlayerDetected)
        {
            // Head to the last known player position
            _agent.SetDestination(_lastKnownPlayerPosition);
            _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 2));

            // Check if AI has reached the last known player position
            if (Vector3.Distance(transform.position, _lastKnownPlayerPosition) < 1f)
            {
                _isPlayerLastPositionKnown = false;
                currentState = State.Idling;
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Is in attack state.");

        _agent.isStopped = true;

        if (_timeSinceLastAttack >= timeBetweenAttacks)
        {
            TriggerAttack();
        }
        
        CheckPlayerDistance();
    }
    
    private void CheckPlayerDistance()
    {
        float playerDistance = Vector3.Distance(_playerTransform.position, transform.position);

        if (playerDistance > attackRange)
        {
            // Transition to chase or idle based on player's position
            currentState = (playerDistance <= sightRange) ? State.Chasing : State.Idling;
            _agent.isStopped = false;
        }
    }
    
    private float CalculateAttackChance()
    {
        // Adjust the probability thresholds based on the attack count
        float lightAttackChance = 0.7f - (0.1f * (_attackCount / 3));
        return Mathf.Clamp(lightAttackChance, 0.5f, 0.7f);
    }

    private void TriggerAttack()
    {
        var attackProbability = Random.value;
        if (attackProbability > CalculateAttackChance())
        {
            _animator.SetTrigger("HeavyAttackTrigger");
        }
        else
        {
            _animator.SetTrigger("NormalAttackTrigger");
        }

        // Increment the attack count and reset if it reaches 3
        _attackCount = (_attackCount + 1) % 3;
    }
    
    public void OnAttackAnimationComplete()
    {
        _animator.ResetTrigger("HeavyAttackTrigger");
        _animator.ResetTrigger("NormalAttackTrigger");
        _timeSinceLastAttack = 0f;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        // Reduce health
        health -= damage;
    
        // Check if the enemy is dead
        if (health <= 0)
        {
            currentState = State.Death;
        }
        else
        {
            // Trigger hit animation
            _animator.SetTrigger("HitTrigger");
        }
    }

    private void Death()
    {
        _animator.ResetTrigger("HeavyAttackTrigger");
        _animator.ResetTrigger("NormalAttackTrigger");
        
        _isDead = true;
        _animator.SetBool("IsDead", true);
        
        _agent.enabled = false;
        _collider.enabled = false;
        
        // // Chance to drop loot
        // if (Random.value < 0.5f) // 50% chance to drop loot
        // {
        //     Instantiate(lootPrefab, transform.position, Quaternion.identity);
        // }
    }

    // You can call this method to trigger the enemy spawn externally
    // public void TriggerSpawn()
    // {
    //     currentState = State.InitialSpawn;
    // }
}