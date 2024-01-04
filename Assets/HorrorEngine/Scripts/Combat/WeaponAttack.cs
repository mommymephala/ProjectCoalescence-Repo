namespace HorrorEngine
{
    public abstract class WeaponAttack : AttackBase
    {
        protected Weapon m_Weapon;

        protected HEWeaponData MHeWeaponData => m_Weapon.heWeaponData;

        protected override void Awake()
        {
            base.Awake();
            m_Weapon = GetComponentInParent<Weapon>();
        }
    }
}