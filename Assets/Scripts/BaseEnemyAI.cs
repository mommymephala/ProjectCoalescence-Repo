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
    private CapsuleCollider _collider;
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
    
    private int _consecutiveHeadshots = 0;
    private int _consecutiveNonHeadshots = 0;
    private float _timeSinceLastHeadshot = 0f;
    private const float HeadshotTimeout = 4f;
    private float _timeSinceLastNonHeadshot = 0f;
    private const float NonHeadshotTimeout = 2.5f;
    
    private const float MaxHeadshotStaggerChance = 0.75f;
    private const float MaxNonHeadshotStaggerChance = 0.25f;
    protected bool _isPlayerLastPositionKnown;
    public bool IsSpawn;
    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _health = GetComponent<Health>();
    }

    protected virtual void Start()
    {
        if (IsSpawn)
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
        _timeSinceLastHeadshot += Time.deltaTime;
        _timeSinceLastNonHeadshot += Time.deltaTime;
        if (currentState == State.Attacking)
        {
            // Ensure the enemy stays in place during the attack
            _agent.isStopped = true;
        }
        if (_timeSinceLastHeadshot > HeadshotTimeout)
        {
            _consecutiveHeadshots = 0;
            _timeSinceLastHeadshot = 0f;
        }

        if (_timeSinceLastNonHeadshot > NonHeadshotTimeout)
        {
            _consecutiveNonHeadshots = 0;
            _timeSinceLastNonHeadshot = 0f;
        }
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
    public void SpawnStall()
    {
        _agent.isStopped = true;
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

    // private void InitialSpawnBehavior()
    // {
    //     // Implement your spawn animation logic here
    //     _animator.Play("SpawnAnimation");
    //     currentState = State.Idling;
    // }

    protected virtual void Idle()
    {
        if (_isDead) return;
        _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), 0, Time.deltaTime * 5));
    }

    protected virtual void Chase()
    {
        // Check if the player is detected or if the AI has taken a hit
        if (_isPlayerDetected)
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            FacePlayer(turningSpeed);
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
                _isPlayerLastPositionKnown = false;
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
    
    protected virtual void CheckPlayerDistance()
    {
        var playerDistance = Vector3.Distance(_playerTransform.position, transform.position);

        if (playerDistance > attackRange)
        {
            currentState = (playerDistance <= sightRange) ? State.Chasing : State.Idling;
            // Resume NavMeshAgent to allow movement
            _agent.isStopped = false;
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
        // Adjust the probability thresholds based on the attack count
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

        // Increment the attack count and reset if it reaches 3
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

        // Transition back to appropriate state after attack is complete
        TransitionAfterAttack();
    }

    public virtual void OnNormalAttackComplete()
    {
        _animator.ResetTrigger("NormalAttackTrigger");

        // Transition back to appropriate state after attack is complete
        TransitionAfterAttack();
    }

    private void TransitionAfterAttack()
    {
        // Check if the player is still within the chase range
        if (Vector3.Distance(_playerTransform.position, transform.position) <= chaseDetectionRadius)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idling;
        }

        // Resume the NavMeshAgent based on the new state
        if (currentState == State.Chasing)
        {
            _agent.isStopped = false;
        }
    }
    
    protected virtual float CalculateStaggerChance(bool isHeadshot)
    {
        float staggerChance;
        if (isHeadshot)
        {
            staggerChance = Mathf.Lerp(0.35f, MaxHeadshotStaggerChance, Mathf.Min(_consecutiveHeadshots, 3) / 3f);
        }
        else
        {
            staggerChance = Mathf.Lerp(0.1f, MaxNonHeadshotStaggerChance, Mathf.Min(_consecutiveNonHeadshots, 3) / 3f);
        }

        return staggerChance;
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

        if (isWeakpoint)
        {
            _consecutiveHeadshots++;
            _timeSinceLastHeadshot = 0f;
            _consecutiveNonHeadshots = 0;
            _timeSinceLastNonHeadshot = 0f;
            if (_consecutiveHeadshots > 3) _consecutiveHeadshots = 1;
        }
        else
        {
            _consecutiveNonHeadshots++;
            _timeSinceLastNonHeadshot = 0f;
            _consecutiveHeadshots = 0;
            _timeSinceLastHeadshot = 0f;
            if (_consecutiveNonHeadshots > 3) _consecutiveNonHeadshots = 1;
        }

        // Determine if a stagger should happen
        var staggerChance = CalculateStaggerChance(isWeakpoint);
        if (Random.value < staggerChance)
        {
            TriggerStaggerAnimation();

            // Reset both headshot and non-headshot counters after triggering stagger
            _consecutiveHeadshots = 0;
            _consecutiveNonHeadshots = 0;
        }

        if (_health.IsDead)
        {
            _isDead = true;
            currentState = State.Death;
        }
        
        else
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
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
