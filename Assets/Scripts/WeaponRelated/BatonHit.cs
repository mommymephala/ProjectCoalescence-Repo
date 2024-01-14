using Interfaces;
using UnityEngine;

public class BatonHit : MonoBehaviour
{
    [SerializeField] private float damageAmount;
    private bool isChargedAttack = false;
    [SerializeField] private float extraDamage;

    public void SetChargedAttack(bool charged)
    {
        isChargedAttack = charged;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            float totalDamage = isChargedAttack ? damageAmount + extraDamage : damageAmount;
            damageable.TakeDamage(totalDamage, isChargedAttack); // Pass the charged attack flag
            Debug.Log("Damage applied to enemy: " + totalDamage);

            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"Collider {other.name} is not enemy.");
        }
    }
}
