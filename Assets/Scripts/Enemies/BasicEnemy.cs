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
        private bool _isPlayerFound = false;
        [SerializeField] private float health;
        [SerializeField] private LayerMask player;
        
        // Patrolling
        private Vector3 _walkPoint;
        private bool _walkPointSet;
        [SerializeField] private float walkPointRange;
        [SerializeField] private float startIdleTime;
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
        [SerializeField] private float walkSpeed;
        [SerializeField] private float chaseSpeed;
        
        /*
         *
         *
         *
         * RE-WRITE ONCE ANIMATIONS AND THE MODEL IS CHANGED, CHANGE ALL THAT BOOL CHECKS WITH BETTER CONDITIONALS
         *
         *
         *
         * 
         */

        private enum EnemyState
        {
            Idle,
            Walking,
            Chasing,
            Attacking
        }

        private EnemyState _currentState;
        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int IsLightAttacking = Animator.StringToHash("isLightAttacking");
        private static readonly int IsHeavyAttacking = Animator.StringToHash("isHeavyAttacking");
        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _currentState = EnemyState.Idle;
            _idleTimer = startIdleTime;
        }

        private void Update()
        {
            if (!_isPlayerFound)
            {
                FindPlayer();
                if (_player == null) return; // If the player is still not found, return early
            }
            
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
                    if (_playerInSightRange && _playerInAttackRange)
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
                    if (_playerInSightRange && _playerInAttackRange)
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
        
        private void FindPlayer()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) return;
            _player = playerObject.transform;
            _isPlayerFound = true;
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

            if (!_walkPointSet && _idleTimer > 0)
            {
                _animator.SetBool(IsIdle, true);
                _animator.SetBool(IsRunning, false);
                _animator.SetBool(IsLightAttacking, false);
                _animator.SetBool(IsHeavyAttacking, false);
                _animator.SetBool(IsWalking, false);
            }
        }

        private void Walk()
        {
            _animator.SetBool(IsIdle, false);
            _animator.SetBool(IsRunning, false);
            _animator.SetBool(IsLightAttacking, false);
            _animator.SetBool(IsHeavyAttacking, false);
            _animator.SetBool(IsWalking, true);
        }

        private void ChasePlayer()
        {
            _agent.SetDestination(_player.position);
            var lookAtPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            transform.LookAt(lookAtPosition);
            
            _animator.SetBool(IsIdle, false);
            _animator.SetBool(IsLightAttacking, false);
            _animator.SetBool(IsHeavyAttacking, false);
            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsRunning, true);
        }

        private void AttackPlayer()
        {
            var lookAtPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            transform.LookAt(lookAtPosition);
            
            _animator.SetBool(IsIdle, false);
            _animator.SetBool(IsRunning, false);
            _animator.SetBool(IsWalking, false);

            if (!_alreadyAttacked)
            {
                var randomAttack = Random.Range(0f, 1f);
                if (randomAttack < 0.6f)
                {
                    // Light attack code here
                    _animator.SetBool(IsLightAttacking, true);
                    _animator.SetBool(IsHeavyAttacking, false);
                    Debug.Log("Light Attack");
                }
                else
                {
                    // Heavy attack code here
                    _animator.SetBool(IsLightAttacking, false);
                    _animator.SetBool(IsHeavyAttacking, true);
                    Debug.Log("Heavy Attack");
                }

                _alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
            _animator.SetBool(IsLightAttacking, false);
            _animator.SetBool(IsHeavyAttacking, false);
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
            if (health <= 0) Die();
        }

        private void Die()
        {
            /*// Disable NavMeshAgent and Animator
            _agent.enabled = false;
            _animator.enabled = false;

            // Enable the Rigidbody on all ragdoll parts
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }

            // Optional: Add force to ragdoll parts for impact effect
            // You can modify this based on the hit direction and power
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.AddForce(Vector3.back * 50f, ForceMode.Impulse);
            }*/

            Destroy(gameObject, 1f);
        }

    }
}