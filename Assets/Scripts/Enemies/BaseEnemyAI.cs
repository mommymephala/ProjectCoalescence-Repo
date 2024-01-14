using System;
using HorrorEngine;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BaseEnemyAI : MonoBehaviour, IDamageable
{
    public enum State
    {
        SpawnStall,
        InitialSpawn,
        Idling,
        Chasing,
        Attacking,
        Death
    }

    [HideInInspector] public State currentState;
    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected CapsuleCollider _collider;
    protected Transform _playerTransform;

    protected bool _isPlayerDetected = false;
    protected Vector3 _lastKnownPlayerPosition;
    
    [SerializeField] private float idleDetectionRadius = 4f;
    [SerializeField] private float chaseDetectionRadius = 8f;
    [SerializeField] private EnemyHitBox normalHitBox;
    [SerializeField] private EnemyHitBox heavyHitBox;
    public LayerMask playerLayer;

    public float timeBetweenAttacks = 2f;
    public float turningSpeed = 5f;
    private float _timeSinceLastAttack = 0f;

    protected bool _isDead = false;
    private bool _hasTakenHit = false;
    private Health _health;

    public float sightRange = 10f;
    public float attackRange = 2f;
    private int _attackCount = 0;
    
    protected bool IsPlayerLastPositionKnown;
    public bool isSpawn;
    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<Health>();
    }

    protected virtual void Start()
    {
        if (isSpawn)
        {
            currentState = State.SpawnStall;
        }
        
        else
        {
            currentState = State.Idling;
        }
        
        normalHitBox.gameObject.SetActive(false);
        heavyHitBox.gameObject.SetActive(false);
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    protected virtual void Update()
    {
        _timeSinceLastAttack += Time.deltaTime;
        
        if (currentState == State.Attacking)
        {
            _agent.isStopped = true;
        }
        
        _animator.SetBool("IsSpawn", isSpawn);
        DetectPlayer();

        switch (currentState)
        {
            case State.SpawnStall:
                SpawnStall();
                break;
            case State.InitialSpawn:
                SpawnBehavior();
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

    private void SpawnStall()
    {
        _agent.isStopped = true;
        _collider.enabled = false;
    }
    
    protected virtual void DetectPlayer()
    {
        if (currentState == State.SpawnStall)
        {
            _agent.isStopped = true;
        }
        
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
                if (currentState == State.SpawnStall)
                {
                    Debug.Log("içerdeyim");
                    currentState = State.InitialSpawn;
                }
                if (currentState != State.Attacking && currentState != State.Chasing && currentState != State.SpawnStall && currentState != State.InitialSpawn)
                {
                    currentState = State.Chasing;
                }
                return;
            }
        }
    }

    protected virtual void Idle()
    {
        if (_isDead) return;
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), 0, Time.deltaTime * 5));
    }

    protected virtual void Chase()
    {
        if (_isPlayerDetected)
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            IsPlayerLastPositionKnown = true;
            FacePlayer(turningSpeed);
            MoveTowardsPlayer();

            var distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);

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
            
            if (Vector3.Distance(_playerTransform.position, transform.position) <= attackRange)
            {
                currentState = State.Attacking;
            }
        }
        else if (IsPlayerLastPositionKnown)
        {
            SeekLastKnownPlayerPosition();
        }
    }
    
    protected virtual void MoveTowardsPlayer()
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
    
    protected virtual void SeekLastKnownPlayerPosition()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_lastKnownPlayerPosition);

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                IsPlayerLastPositionKnown = false;
                currentState = State.Idling;
                _animator.SetFloat("Speed", 0);
            }
        }
        else
        {
            _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
        }
    }

    protected virtual void UpdateMovementAnimation(float distanceToPlayer)
    {
        var normalizedSpeed = Mathf.InverseLerp(attackRange, sightRange, distanceToPlayer);
        _agent.speed = Mathf.Lerp(1f, 0.5f, normalizedSpeed);
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));
    }
    
    protected virtual void FacePlayer(float turningSpeed)
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
        }
    }

    protected virtual void Attack()
    {
        // Lock the enemy's position by stopping the NavMeshAgent
        _agent.isStopped = true;

        // Face the player without moving
        FacePlayer(turningSpeed);

        if (_timeSinceLastAttack >= timeBetweenAttacks)
        {
            _timeSinceLastAttack = 0f;
            _animator.SetFloat("Speed", 0);
            TriggerAttack();
        }
    }
    
    protected virtual float CalculateAttackChance()
    {
        var lightAttackChance = 0.7f - (0.1f * (_attackCount / 3));
        return Mathf.Clamp(lightAttackChance, 0.5f, 0.7f);
    }

    protected virtual void TriggerAttack()
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

        _attackCount = (_attackCount + 1) % 3;
    }
    
    public virtual void EnableNormalHitBox()
    {
        if (normalHitBox != null)
        {
            normalHitBox.gameObject.SetActive(enabled);
        }
    }

    public virtual void DisableNormalHitBox()
    {
        if (normalHitBox != null)
        {
            normalHitBox.gameObject.SetActive(false);
        }
    }
    
    public virtual void EnableHeavyHitBox()
    {
        if (heavyHitBox != null)
        {
            heavyHitBox.gameObject.SetActive(true);
        }
    }

    public virtual void DisableHeavyHitBox()
    {
        if (heavyHitBox != null)
        {
            heavyHitBox.gameObject.SetActive(false);
        }
    }
    
    public virtual void OnHeavyAttackComplete()
    {
        _animator.ResetTrigger("HeavyAttackTrigger");

        TransitionAfterAttack();
    }

    public virtual void OnNormalAttackComplete()
    {
        _animator.ResetTrigger("NormalAttackTrigger");

        TransitionAfterAttack();
    }

    private void TransitionAfterAttack()
    {
        if (Vector3.Distance(_playerTransform.position, transform.position) <= chaseDetectionRadius)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idling;
        }

        if (currentState == State.Chasing)
        {
            _agent.isStopped = false;
        }
    }
    
    protected virtual void TriggerStaggerAnimation()
    {
        _animator.SetTrigger("HitTrigger");
    }

    public virtual void TakeDamage(float damage, bool isChargedAttack, bool isWeakpoint)
    {
        if (_isDead) return;

        _health.DamageReceived(damage);
        _hasTakenHit = true;

        TriggerStaggerAnimation();

        if (_health.IsDead)
        {
            _isDead = true;
            currentState = State.Death;
        }
        else
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            IsPlayerLastPositionKnown = true;
            _isPlayerDetected = true;
            currentState = State.Chasing;
            FacePlayer(turningSpeed);
        }
    }

    protected virtual void Death()
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
    protected virtual void SpawnBehavior()
    {
        // Base class does nothing
    }
    // You can call this method to trigger the enemy spawn externally
    // public void TriggerSpawn()
    // {
    //     currentState = State.InitialSpawn;
    // }
}
