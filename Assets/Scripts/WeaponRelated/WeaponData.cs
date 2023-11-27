using UnityEngine;

namespace WeaponRelated
{
    [CreateAssetMenu(fileName="Weapon", menuName="Weapon")]
    public class WeaponData : ScriptableObject 
    {
        //[Header("Info")]
        // public Sprite weaponIcon;
        // public GameObject weaponPrefab;

        [Header("Shooting")]
        public bool allowAutoFire;
        public float damage;
        public float maxDistance;
        [Tooltip("In RPM")] public float fireRate;
        public float spread;
        public int bulletsPerShot;

        [Header("Aim Down Sight")]
        public float originalFOV = 90f;
        public float aimDownSightFOV = 40f;
        public float aimDownSightSpeed = 10f;
        public float zoomSpeed = 5f;
        public float adsOffsetX = 10f;
        public float adsOffsetY = 10f;
        public float adsOffsetZ = 10f;
    
        [Header("Reloading")]
        public int currentAmmo;
        public int magSize;
        public float reloadTime;

        [Header("Recoil Settings")]
        public Vector3 recoilRotationHipfire;
        public Vector3 recoilRotationAiming;
        public float rotationSpeed = 6;
        public float returnSpeed = 25;
        public float kickbackForce = 0.05f;
        public float kickbackRotation = 2f;
        public float kickbackDuration = 0.1f;
        public float resetDuration = 0.2f;
        public float walkingRecoilMultiplier = 1.5f;
        public float sprintingRecoilMultiplier = 2f;
    }
}