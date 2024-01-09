using UnityEngine;

namespace Interfaces
{
    public interface IDamageable 
    {
        public void TakeDamage(float damage, Vector3 impactPoint, Vector3 impactDir);
    }
}
