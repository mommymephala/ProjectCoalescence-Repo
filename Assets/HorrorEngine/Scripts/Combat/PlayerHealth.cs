using Interfaces;

namespace HorrorEngine
{
    public class PlayerHealth : Health, IDamageable
    {
        public void TakeDamage(float damage, bool isChargedAttack, bool isWeakpoint)
        {
            AudioManager.Instance.PlayPlayerTakeDamage();
            DamageReceived(damage);
        }
    }
}