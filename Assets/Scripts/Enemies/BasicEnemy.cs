using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class BasicEnemy : MonoBehaviour, IDamageable
    {
        private NavMeshAgent _agent;
        private Transform _player;
        [SerializeField] private float health;
        [SerializeField] private LayerMask player;
        
        // Patrolling
        private Vector3 _walkPoint;
        private bool _walkPointSet;
        [SerializeField] private float walkPointRange;
        [SerializeField] private float timeBetweenIdleAndWalk;
        private float _idleTimer;

        // Attacking
        [SerializeField] private float timeBetweenAttacks;
        private bool _alreadyAttacked;

        // States
        [SerializeField] private float sightRange, attackRange;
        private bool _playerInSightRange, _playerInAttackRange;

        // Animator
        private Animator _animator;
        private bool _isLightAttacking;
        private bool _isHeavyAttacking;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float chaseSpeed = 5f;

        private enum EnemyState
        {
            Idle,
            Walking,
            Chasing,
            Attacking
        }

        private EnemyState _currentState;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _currentState = EnemyState.Idle;
            _idleTimer = timeBetweenIdleAndWalk;
        }

        private void Update()
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, sightRange, player);
            _playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, player);

            switch (_currentState)
            {
                case EnemyState.Idle:
                    Idle();

                    if (_idleTimer <= 0 && !_walkPointSet)
                    {
                        SearchWalkPoint();
                        _currentState = EnemyState.Walking;
                    }
                    if (_playerInSightRange && !_playerInAttackRange)
                    {
                        _currentState = EnemyState.Chasing;
                    }
                    break;
                
                case EnemyState.Walking:
                    
                    _agent.speed = walkSpeed;
                    _agent.SetDestination(_walkPoint);
                    _walkPointSet = true;
                    Vector3 distanceToWalkPoint = transform.position - _walkPoint;
                    Walk();
                    if (distanceToWalkPoint.magnitude < 1f)
                    {
                        _walkPointSet = false;
                        _idleTimer = timeBetweenIdleAndWalk;
                        _currentState = EnemyState.Idle;
                    }
                    if (_playerInSightRange && !_playerInAttackRange)
                    {
                        _currentState = EnemyState.Chasing;
                    }
                    break;

                case EnemyState.Chasing:
                    
                    _agent.speed = chaseSpeed;
                    ChasePlayer();

                    if (_playerInAttackRange)
                    {
                        _currentState = EnemyState.Attacking;
                    }
                    if(!_playerInAttackRange)
                    {
                        _currentState = EnemyState.Chasing;
                    }
                    if(!_playerInAttackRange && !_playerInSightRange)
                    {
                        _currentState = EnemyState.Chasing;
                    }
                    break;

                case EnemyState.Attacking:
                    
                    AttackPlayer();
                    
                    if (!_playerInAttackRange)
                    {
                        _currentState = EnemyState.Chasing;
                    }
                    if (_playerInAttackRange)
                    {
                        _currentState = EnemyState.Attacking;
                    }
                    if(!_playerInAttackRange && !_playerInSightRange)
                    {
                        _currentState = EnemyState.Chasing;
                    }
                    break;
            }
        }
        
        private void SearchWalkPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
            randomDirection += transform.position;
            
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkPointRange, NavMesh.AllAreas))
            {
                _walkPointSet = true;
                _walkPoint = hit.position;
            }
        }
        
        private void Idle()
        {
            _idleTimer -= Time.deltaTime;
            Debug.Log(_idleTimer);

            if (!_walkPointSet && _idleTimer > 0)
            {
                _animator.SetBool("isIdle", true);
                _animator.SetBool("isRunning", false);
                _animator.SetBool("isLightAttacking", false);
                _animator.SetBool("isHeavyAttacking", false);
                _animator.SetBool("isWalking", false);
            }
        }

        private void Walk()
        {
            _animator.SetBool("isIdle", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isLightAttacking", false);
            _animator.SetBool("isHeavyAttacking", false);
            _animator.SetBool("isWalking", true);
        }

        private void ChasePlayer()
        {
            _agent.SetDestination(_player.position);
            var lookAtPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            transform.LookAt(lookAtPosition);
            
            _animator.SetBool("isIdle", false);
            _animator.SetBool("isLightAttacking", false);
            _animator.SetBool("isHeavyAttacking", false);
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", true);
        }

        private void AttackPlayer()
        {
            var lookAtPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            transform.LookAt(lookAtPosition);
            
            _animator.SetBool("isIdle", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isWalking", false);

            if (!_alreadyAttacked)
            {
                var randomAttack = Random.Range(0f, 1f);
                if (randomAttack < 0.75f)
                {
                    // Light attack code here
                    _animator.SetBool("isLightAttacking", true);
                    _animator.SetBool("isHeavyAttacking", false);
                    Debug.Log("Light Attack");
                }
                else
                {
                    // Heavy attack code here
                    _animator.SetBool("isLightAttacking", false);
                    _animator.SetBool("isHeavyAttacking", true);
                    Debug.Log("Heavy Attack");
                }

                _alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
            _animator.SetBool("isLightAttacking", false);
            _animator.SetBool("isHeavyAttacking", false);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, walkPointRange);
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0) Destroy(gameObject);
        }
    }
}