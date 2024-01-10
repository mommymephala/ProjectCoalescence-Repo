using UnityEngine;

namespace HorrorEngine
{
    public class EnemyHitBox : MonoBehaviour
    {
        [SerializeField] private float damageAmount;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damageAmount, false, false);
                Debug.Log("Damage applied to player: " + damageAmount);

                gameObject.SetActive(false);
            }
            else
            {
                // Debug.Log($"Collider {other.name} is not a PlayerHealth.");
            }
        }
    }
}