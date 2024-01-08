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
    
    [SerializeField] private SphereCollider detectionSphere;
    public float idleDetectionRadius = 5f;
    public float chaseDetectionRadius = 10f;
    
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
    
    // public LayerMask playerLayer;
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
        detectionSphere.radius = idleDetectionRadius;
    }

    private void Update()
    {
        if (_playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }
        
        if (_isDead) return;
        _timeSinceLastAttack += Time.deltaTime;

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
        detectionSphere.radius = idleDetectionRadius;
        
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), 0, Time.deltaTime * 5));
        
    }

    private void Chase()
    {
        if (_isPlayerDetected)
        {
            Debug.Log("Is in chase state.");
            detectionSphere.radius = chaseDetectionRadius;
        
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;

            var distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);

            // Continue chasing if the player is within sight range
            if (distanceToPlayer <= sightRange && distanceToPlayer > attackRange)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_playerTransform.position);

                var normalizedSpeed = Mathf.InverseLerp(attackRange, sightRange, distanceToPlayer);
                _agent.speed = Mathf.Lerp(1f, 0.5f, normalizedSpeed);
                _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
            }
            else if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attacking;
            }
        }
        else if (_isPlayerLastPositionKnown)
        {
            // Head to the last known player position
            _agent.isStopped = false;
            _agent.SetDestination(_lastKnownPlayerPosition);

            if (Vector3.Distance(transform.position, _lastKnownPlayerPosition) < 1f)
            {
                // Once reached the last known position, stop chasing
                _isPlayerLastPositionKnown = false;
                currentState = State.Idling;
                _animator.SetFloat("Speed", 0); // Stop the chase animation
            }
            else
            {
                // Continue the chase animation while moving towards the last known position
                _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerDetected = true;
            _playerTransform = other.transform;
            currentState = State.Chasing;
        }
    }
    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerDetected = true;
            _playerTransform = other.transform;
            currentState = State.Chasing;
        }
    }*/
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerDetected = false;
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            currentState = State.Chasing;
        }
    }

    private void Attack()
    {
        Debug.Log("Is in attack state.");
        CheckPlayerDistance();
        FacePlayer();

        if (_timeSinceLastAttack >= timeBetweenAttacks)
        {
            _timeSinceLastAttack = 0f;
            _animator.SetFloat("Speed",  0);
            TriggerAttack();
        }
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        // Ensure the rotation is only on the y-axis
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    
    private void CheckPlayerDistance()
    {
        var playerDistance = Vector3.Distance(_playerTransform.position, transform.position);

        if (playerDistance > attackRange)
        {
            currentState = (playerDistance <= sightRange) ? State.Chasing : State.Idling;
            _agent.isStopped = false; // Ensure the agent can move again
        }
    }
    
    private float CalculateAttackChance()
    {
        // Adjust the probability thresholds based on the attack count
        var lightAttackChance = 0.7f - (0.1f * (_attackCount / 3));
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
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        // Reduce health
        health -= damage;

        // Update last known player position when taking damage
        if (_playerTransform != null)
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            detectionSphere.radius = chaseDetectionRadius;
            currentState = State.Chasing;
        }

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