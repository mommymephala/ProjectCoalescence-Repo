using UnityEngine;

namespace Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        public Collider lightHitbox;
        public Collider heavyHitbox;

        private void Start()
        {
            lightHitbox.enabled = false;
            heavyHitbox.enabled = false;
        }

        public void EnableLightHitbox()
        {
            lightHitbox.enabled = true;
        }

        public void DisableLightHitbox()
        {
            lightHitbox.enabled = false;
        }

        public void EnableHeavyHitbox()
        {
            heavyHitbox.enabled = true;
        }

        public void DisableHeavyHitbox()
        {
            heavyHitbox.enabled = false;
        }
    }
}