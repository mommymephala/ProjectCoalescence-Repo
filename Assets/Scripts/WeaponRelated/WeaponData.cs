using FMODUnity;
using HorrorEngine;
using UnityEngine;
//using FMODUnity;
using FMOD.Studio;
namespace WeaponRelated
{
    [CreateAssetMenu(fileName="Weapon", menuName="Weapon")]
    
    public class WeaponData : ReloadableHEWeaponData
    {
        [Header("Audio")] 
        public EventReference gunShotSFX;
        public EventReference ReloadSFX;
        public EventReference OutOfAmmo;
        public EventInstance gunshotInstance;
        public EventReference gunSurfaceEffect;
        [Header("Surface Hit Sounds")]
        //public EventReference wallHitSFX;
        //public EventReference targetHitSFX;
        
        [Header("Shooting")]
        public bool allowAutoFire;
        public float damage;
        public float maxDistance;
        
        [Header("Crosshair")]
        public GameObject crosshairPrefab;
        
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
        // public int currentAmmo;
        // public int maxAmmo;
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
    }
}