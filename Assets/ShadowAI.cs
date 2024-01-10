using System;
using HorrorEngine;
using Interfaces;
using UnityEngine;

public class ShadowAI : MonoBehaviour, IDamageable
{
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    public void TakeDamage(float damage, bool isChargedAttack, bool isHeadshot)
    {
        // Only apply damage if the attack is charged
        if (isChargedAttack)
        {
            _health.DamageReceived(damage);
        }
    }
}
