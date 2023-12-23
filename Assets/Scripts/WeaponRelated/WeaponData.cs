using UnityEngine;
using FMODUnity;
using FMOD.Studio;
namespace WeaponRelated
{
   
    
    [CreateAssetMenu(fileName="Weapon", menuName="Weapon")]
    
    public class WeaponData : ScriptableObject 
    {
        [Header("Audio")]
        [SerializeField] private EventReference deagledeneme2;
       //[SerializeField] private EventReference Rifle;
       // [SerializeField] private EventReference Shotgun;
        [Header("Shooting")]
        public bool allowAutoFire;
        public float damage;
        public float maxDistance;
        [Tooltip("In RPM")] public float fireRate;
        public float spread;
        public int bulletsPerShot;

        [Header("Aim Down Sight")]
        public float originalPlayerFOV;
        public float originalWeaponFOV;
        public float aimDownSightFOV;
        public float aimDownSightSpeed;
        public float zoomSpeed;
    
        [Header("Reloading")]
        public int currentAmmo;
        public int magSize;
        public float reloadTime;

        [Header("Recoil Settings")]
        public Vector3 recoilRotationHipfire;
        public Vector3 recoilRotationAiming;
        public float rotationSpeed;
        public float smoothingFactor;
        public float returnSpeed;
        public float kickbackForce;
        public float kickbackDuration;
        public float resetDuration;
        public float walkingRecoilMultiplier;
        
        //public float sprintingRecoilMultiplier;
        
        
        public void PlayDeagleRanged()
        {
            if (deagledeneme2.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playerAttackRanged");
                return;
            }
            RuntimeManager.PlayOneShot(deagledeneme2);
       
            Debug.Log("deagle ses");
        }
       /* public void PlayRifleRanged()
        {
            if (deagledeneme2.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playerAttackRanged");
                return;
            }
            RuntimeManager.PlayOneShot(deagledeneme2);
       
            Debug.Log("deagle ses");
        }
        public void PlayShotgunRanged()
        {
            if (deagledeneme2.IsNull)
            {
                Debug.LogWarning("Fmod event not found: playerAttackRanged");
                return;
            }
            RuntimeManager.PlayOneShot(deagledeneme2);
       
            Debug.Log("shotgun ses");
        }*/
    }
}