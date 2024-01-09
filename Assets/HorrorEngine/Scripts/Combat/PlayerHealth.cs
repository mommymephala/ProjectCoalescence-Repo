using Interfaces;

namespace HorrorEngine
{
    public class PlayerHealth : Health, IDamageable
    {
        public void TakeDamage(float damage)
        {
            DamageReceived(damage);
        }
    }
}