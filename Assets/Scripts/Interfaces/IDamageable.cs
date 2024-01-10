namespace Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float damage, bool isChargedAttack, bool isHeadshot = false);
    }
}
