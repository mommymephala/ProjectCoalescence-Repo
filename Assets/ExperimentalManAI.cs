using UnityEngine;

public class ExperimentalManAI : BaseEnemyAI
{
    protected override void Chase()
    {
        if (_isDead) return;

        if (_isPlayerDetected)
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            _isPlayerLastPositionKnown = true;
            FacePlayer(turningSpeed);
        
            var distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);

            var normalizedSpeed = Mathf.InverseLerp(attackRange, sightRange, distanceToPlayer);
            _agent.speed = Mathf.Lerp(0.5f, 1f, normalizedSpeed);
            _animator.SetFloat("Speed", Mathf.Lerp(_animator.GetFloat("Speed"), _agent.speed, Time.deltaTime * 5));

            if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attacking;
                _agent.isStopped = true;
            }
            else
            {
                _agent.isStopped = false;
                _agent.SetDestination(_playerTransform.position);
            }
        }
        else if (_isPlayerLastPositionKnown)
        {
            SeekLastKnownPlayerPosition();
        }
    }
    
    public override void TakeDamage(float damage, bool isChargedAttack, bool isWeakpoint)
    {
        if (!isWeakpoint) return;
        base.TakeDamage(damage, isChargedAttack, true);

    }
}