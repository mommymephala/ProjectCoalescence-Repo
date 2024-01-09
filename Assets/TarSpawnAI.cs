using System;
using HorrorEngine;
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
    private Vector3 _lastKnownPlayerPosition;
    
    [SerializeField] private float idleDetectionRadius = 4f;
    [SerializeField] private float chaseDetectionRadius = 8f;
    [SerializeField] private EnemyHitBox normalHitBox;
    [SerializeField] private EnemyHitBox heavyHitBox;
    public LayerMask playerLayer;

    public float timeBetweenAttacks = 2f;
    private float _timeSinceLastAttack = 0f;

    private bool _isDead = false;
    private bool _hasTakenHit = false;
    private Health _health;

    public float sightRange = 10f;
    public float attackRange = 2f;
    private int _attackCount = 0;
    private bool _isPlayerLastPositionKnown;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        currentState = State.Idling;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    private void Update()
    {
        _timeSinceLastAttack += Time.deltaTime;
        DetectPlayer();

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

    private void DetectPlayer()
    {
        if (_isDead) return;
        if (currentState == State.Idling && _hasTakenHit)
        {
            _isPlayerDetected = true;
            currentState = State.Chasing;
            _hasTakenHit = false;
        }
        
        var detectionRadius = currentState == State.Idling ? idleDetectionRadius : chaseDetectionRadius;
        var hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        _isPlayerDetected = false;
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Player"))
            {
                _isPlayerDetected = true;
                _playerTransform = hitCollider.transform;
                
                if (currentState != State.Attacking && currentState != State.Chasing)
                {
                    currentState = State.Chasing;
                }
                return;
            }
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
        if (_isDead) return;
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), 0, Time.deltaTime * 5));
    }

    private void Chase()
    {
        // Check if the player is detected or if the AI has taken a hit
        if (_isPlayerDetected)
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            FacePlayer();
            MoveTowardsPlayer();

            var distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);

            if (distanceToPlayer <= sightRange && distanceToPlayer > attackRange)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_playerTransform.position);

                // Normalize speed based on distance to player
                var normalizedSpeed = Mathf.InverseLerp(attackRange, sightRange, distanceToPlayer);
                _agent.speed = Mathf.Lerp(1f, 0.5f, normalizedSpeed);
                _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
            }
            
            else if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attacking;
            }
            
            if (Vector3.Distance(_playerTransform.position, transform.position) <= attackRange)
            {
                currentState = State.Attacking;
            }
        }
        else if (_isPlayerLastPositionKnown)
        {
            SeekLastKnownPlayerPosition();
        }
        // else
        // {
        //     // Transition back to Idle if the player is lost and not in last known position
        //     currentState = State.Idling;
        // }
    }
    
    private void MoveTowardsPlayer()
    {
        var distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);
        if (distanceToPlayer <= sightRange && distanceToPlayer > attackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_playerTransform.position);
            UpdateMovementAnimation(distanceToPlayer);
        }
        else if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
        }
    }
    
    private void SeekLastKnownPlayerPosition()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_lastKnownPlayerPosition);

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                // Arrived at the last known position
                _isPlayerLastPositionKnown = false;
                currentState = State.Idling;
                _animator.SetFloat("Speed", 0); // Stop the walking animation
            }
        }
        else
        {
            // Still moving towards the last known position
            _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
        }
    }

    private void UpdateMovementAnimation(float distanceToPlayer)
    {
        var normalizedSpeed = Mathf.InverseLerp(attackRange, sightRange, distanceToPlayer);
        _agent.speed = Mathf.Lerp(1f, 0.5f, normalizedSpeed);
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
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

    private void Attack()
    {
        CheckPlayerDistance();

        _agent.isStopped = true;

        if (_timeSinceLastAttack >= timeBetweenAttacks)
        {
            _timeSinceLastAttack = 0f;
            _animator.SetFloat("Speed",  0);
            TriggerAttack();
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
    
    public void EnableNormalHitBox()
    {
        if (normalHitBox != null)
        {
            normalHitBox.enabled = true;
            normalHitBox.ApplyDamage();
        }
    }

    public void DisableNormalHitBox()
    {
        if (normalHitBox != null)
        {
            normalHitBox.enabled = false;
        }
    }
    
    public void EnableHeavyHitBox()
    {
        if (heavyHitBox != null)
        {
            heavyHitBox.enabled = true;
            heavyHitBox.ApplyDamage();
        }
    }

    public void DisableHeavyHitBox()
    {
        if (heavyHitBox != null)
        {
            heavyHitBox.enabled = false;
        }
    }
    
    // Reset animations separately
    public void OnHeavyAttackComplete()
    {
        _animator.ResetTrigger("HeavyAttackTrigger");
        // _animator.ResetTrigger("NormalAttackTrigger");
    }
    
    public void OnNormalAttackComplete()
    {
        // _animator.ResetTrigger("HeavyAttackTrigger");
        _animator.ResetTrigger("NormalAttackTrigger");
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _health.DamageReceived(damage);
        _hasTakenHit = true;

        if (_health.IsDead)
        {
            _isDead = true;
            currentState = State.Death;
        }
        
        else
        {
            _animator.SetTrigger("HitTrigger");

            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            _isPlayerDetected = true;
            currentState = State.Chasing;
            FacePlayer();

        }
    }

    private void Death()
    {
        _isPlayerDetected = false;
        _animator.ResetTrigger("HeavyAttackTrigger");
        _animator.ResetTrigger("NormalAttackTrigger");
        
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
