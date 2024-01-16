using Unity.VisualScripting;
using UnityEngine;

public class ExperimentalManAI : BaseEnemyAI
{
    protected override void Chase()
    {
        if (_isDead) return;

        if (_isPlayerDetected)
        {
            _lastKnownPlayerPosition = _playerTransform.position;
            IsPlayerLastPositionKnown = true;
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
        else if (IsPlayerLastPositionKnown)
        {
            SeekLastKnownPlayerPosition();
        }
    }

    public override void PlayChaseSound()
 {
     AudioManager.Instance.PlayEnemyIdle(gameObject, AudioManager.EnemyType.ChipEnemy);
     
 }
    public override void PlayFootstepSound()
    {
        AudioManager.Instance.PlayEnemyFootStep(gameObject, AudioManager.EnemyType.ChipEnemy);
    }
   
    public override void PlayNormalAttackSound()
    {
        AudioManager.Instance.PlayEnemyAttack(gameObject, AudioManager.EnemyType.ChipEnemy, AudioManager.AttackType.NormalAttack);
    }
    public override void PlayHeavyAttackSound()
    {
        AudioManager.Instance.PlayEnemyAttack(gameObject, AudioManager.EnemyType.ChipEnemy, AudioManager.AttackType.HeavyAttack);
    }
    public override void TakeDamage(float damage, bool isChargedAttack, bool isWeakpoint)
    {
        if (!isWeakpoint) return;
        base.TakeDamage(damage, isChargedAttack, true);
    }
    public override void Death()
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

    public override void PlayOnDeath()
    {
        
            AudioManager.Instance.PlayEnemyDeath(gameObject,AudioManager.EnemyType.ChipEnemy);
        
    }
}