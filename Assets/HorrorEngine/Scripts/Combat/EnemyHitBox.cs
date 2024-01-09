using UnityEngine;

namespace HorrorEngine
{
    public class EnemyHitBox : MonoBehaviour
    {
        [SerializeField] private Vector3 m_Center;
        [SerializeField] private Vector3 m_Size = Vector3.one;
        [SerializeField] private LayerMask m_LayerMask;
        [SerializeField] private float damageAmount;

        public void ApplyDamage()
        {
            var scaledCenter = transform.position + new Vector3(m_Center.x * transform.lossyScale.x, m_Center.y * transform.lossyScale.y, m_Center.z * transform.lossyScale.z);
            var scaledSize = new Vector3(m_Size.x * transform.lossyScale.x, m_Size.y * transform.lossyScale.y, m_Size.z * transform.lossyScale.z);

            var overlapResults = Physics.OverlapBox(scaledCenter, scaledSize * 0.5f, transform.rotation, m_LayerMask, QueryTriggerInteraction.Collide);

            Debug.Log($"OverlapBox found {overlapResults.Length} colliders."); // Debugging line

            foreach (Collider collider in overlapResults)
            {
                if (collider.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log("Damage applied to player: " + damageAmount); // Debugging line
                }
                else
                {
                    Debug.Log($"Collider {collider.name} is not a PlayerHealth."); // Debugging line
                }
            }
        }

        // --------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.red;

                var scaledCenter = new Vector3(m_Center.x * transform.lossyScale.x, m_Center.y * transform.lossyScale.y, m_Center.z * transform.lossyScale.z);
                var scaledSize = new Vector3(m_Size.x * transform.lossyScale.x, m_Size.y * transform.lossyScale.y, m_Size.z * transform.lossyScale.z);

                Gizmos.matrix = Matrix4x4.Rotate(transform.rotation);
                Gizmos.DrawWireCube(transform.position + scaledCenter, scaledSize);
                Gizmos.matrix = Matrix4x4.identity;

                Gizmos.color = Color.white;
            }
        }
    }
}