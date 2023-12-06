using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Interfaces;
using PlayerActions;

namespace WeaponRelated
{
    public enum WeaponType
    {
        Deagle,
        Rifle,
        Shotgun
    }
    public class Weapon : MonoBehaviour
    {
        public WeaponType weaponType;
        [Header("Audio Events")]
        [SerializeField] private string deagleShotEvent;
        [SerializeField] private string rifleShotEvent;
        [SerializeField] private string shotgunShotEvent;
       [SerializeField] private AudioMenager audioMenager;
       
        [Header("References")]
        public WeaponData weaponData;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private bool toggleAimDownSight = true;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera weaponCamera;
        
        [Header("Transforms")]
        [SerializeField] private Transform playerCameraTransform;
        [SerializeField] private Transform weaponCameraTransform;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Transform adsPositionRef;
        
        [Header("Key Codes")]
        [SerializeField] private KeyCode aimDownSightKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode reloadKey = KeyCode.R;
        
        [Header("Visuals")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private GameObject bloodFXPrefab;
        [SerializeField] private GameObject bulletHolePrefab;

        [Header("UI")]
        [SerializeField] private Text currentAmmoText;

        //Flags
        private bool _shooting;
        private bool _reloading;
        private bool _isbloodFXPrefabNull;
        private bool _isbulletHolePrefabNull;
        private bool _ismuzzleFlashPrefabNull;

        //Shooting & ADS variables
        private bool _aimingDownSight;
        private Vector3 _originalAdsLocalPosition;
        private float _timeSinceLastShot;
        private float _targetFOV;
        private float _targetWeaponFOV;
    
        // Properties to access recoil-related variables
        private Vector3 _currentPlayerCameraRotation;
        private Vector3 _currentWeaponCameraRotation;
        private Vector3 _smoothRotation;
        private Vector3 _weaponOriginalLocalPosition;

        private void Awake()
        {
            _ismuzzleFlashPrefabNull = muzzleFlashPrefab == null;
            _isbulletHolePrefabNull = bulletHolePrefab == null;
            _isbloodFXPrefabNull = bloodFXPrefab == null;
            _timeSinceLastShot = weaponData.fireRate / 60f;
            _weaponOriginalLocalPosition = transform.localPosition;
            _originalAdsLocalPosition = adsPositionRef.localPosition;
            weaponData.originalPlayerFOV = playerCamera.fieldOfView;
            weaponData.originalWeaponFOV = weaponCamera.fieldOfView;
        }
        private void Start()
        {
            UpdateAiming(false);
        }

        private void OnDisable()
        {
            _shooting = false;
            _reloading = false;
            _aimingDownSight = false;
            ResetFOV();
        }

        private void Update()
        {
            HandleReloadingInput();
            HandleAdsInput();
            HandleShootingInput();
            Aiming();
            UpdateAmmoUI();
            _timeSinceLastShot += Time.deltaTime;
        }

        private void LateUpdate()
        {
            ApplyRecoil();
        }

        private void HandleShootingInput()
        {
            _shooting = weaponData.allowAutoFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

            if (_shooting && CanShoot())
            {
                Shoot();
            }
        }

        private bool CanShoot()
        {
            return !_reloading && _timeSinceLastShot >= 1f / (weaponData.fireRate / 60f) && !playerMovement.IsSprinting;
        }

        private void Shoot()
        {
            
            if (weaponData.currentAmmo <= 0)
            {
                StartReload();
                return;
            }

            for (var i = 0; i < weaponData.bulletsPerShot; i++)
            {
                Vector3 spreadDirection = weaponCameraTransform.forward + Random.insideUnitSphere * weaponData.spread;
                if (!Physics.Raycast(weaponCameraTransform.position, spreadDirection, out RaycastHit hitInfo,
                        weaponData.maxDistance)) continue;
                Debug.DrawRay(weaponCameraTransform.position, spreadDirection, Color.red, 5);
                if (hitInfo.transform.CompareTag("Wall"))
                {
                    SpawnBulletHole(hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));
                }
                var damageable = hitInfo.transform.GetComponent<IDamageable>();
                if (damageable == null) continue;
                damageable.TakeDamage(weaponData.damage);
                SpawnBloodParticle(hitInfo.point);
            }

            weaponData.currentAmmo--;
            _timeSinceLastShot = 0f;
            
            OnGunShot();
            CalculateRecoil();
            ApplyProceduralKickback();
        }

        private void CalculateRecoil()
        {
            Vector3 recoilRotation = _aimingDownSight ? weaponData.recoilRotationAiming : weaponData.recoilRotationHipfire;

            var recoilMultiplier = 1f;
            if (playerMovement.IsWalking)
            {
                recoilMultiplier = weaponData.walkingRecoilMultiplier;
            }

            // Apply smooth recoil to the current rotation with the recoil multiplier
            _smoothRotation += new Vector3
            (
                -recoilRotation.x,
                Random.Range(-recoilRotation.y, recoilRotation.y),
                Random.Range(-recoilRotation.z, recoilRotation.z)
            ) * recoilMultiplier;
        }
        
        private void ApplyRecoil()
        {
            // Calculate recoil for player camera
            Vector3 currentPlayerCameraRotation = Vector3.Lerp(_currentPlayerCameraRotation, _smoothRotation, weaponData.rotationSpeed * Time.deltaTime * weaponData.smoothingFactor);
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, weaponData.returnSpeed * Time.deltaTime * weaponData.smoothingFactor);
            playerCameraTransform.localRotation = Quaternion.Euler(currentPlayerCameraRotation);
            
            // Calculate recoil for weapon camera
            Vector3 currentWeaponCameraRotation = Vector3.Lerp(_currentWeaponCameraRotation, _smoothRotation, weaponData.rotationSpeed * Time.deltaTime * weaponData.smoothingFactor);
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, weaponData.returnSpeed * Time.deltaTime * weaponData.smoothingFactor);
            weaponCameraTransform.localRotation = Quaternion.Euler(currentWeaponCameraRotation);

            // Update the stored rotations for future calculations
            _currentPlayerCameraRotation = currentPlayerCameraRotation;
            _currentWeaponCameraRotation = currentWeaponCameraRotation;
        }
        
        private void ApplyProceduralKickback()
        {
            Vector3 kickbackDirection = -Vector3.forward;
            
            var kickbackAmount = Mathf.Lerp(0f, weaponData.kickbackForce, weaponData.kickbackDuration);
            Vector3 kickbackVector = kickbackDirection * kickbackAmount;
            transform.localPosition += kickbackVector;

            StartCoroutine(ResetWeaponPositionAfterKickback());
        }
        
        private IEnumerator ResetWeaponPositionAfterKickback()
        {
            yield return new WaitForSeconds(weaponData.kickbackDuration);

            var elapsedTime = 0f;
            Vector3 startPosition = transform.localPosition;
            while (elapsedTime < weaponData.resetDuration)
            {
                transform.localPosition = Vector3.Lerp(startPosition, _weaponOriginalLocalPosition, elapsedTime / weaponData.resetDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _weaponOriginalLocalPosition;
        }

        private void StartReload()
        {
            if (_reloading || !gameObject.activeSelf || weaponData.currentAmmo >= weaponData.magSize) return;
            _aimingDownSight = false;
            StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            _reloading = true;
            yield return new WaitForSeconds(weaponData.reloadTime);
            weaponData.currentAmmo = weaponData.magSize;
            _reloading = false;
        }

        private void HandleReloadingInput()
        {
            if (Input.GetKeyDown(reloadKey))
            {
                StartReload();
            }
        }

        private void Aiming()
        {
            if (_aimingDownSight)
            {
                // Calculate the target screen position at the center of the screen
                var targetScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

                // Convert the screen position to a world position based on the weapon's distance from the camera
                var distanceFromCamera = Vector3.Distance(adsPositionRef.position, playerCamera.transform.position);
                Vector3 targetWorldPosition = playerCamera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, distanceFromCamera));

                // Convert the world position to a local position relative to the weapon
                Vector3 targetLocalPosition = adsPositionRef.parent.InverseTransformPoint(targetWorldPosition);

                adsPositionRef.localPosition = Vector3.Lerp(adsPositionRef.localPosition, targetLocalPosition, Time.deltaTime * weaponData.aimDownSightSpeed);

                // Smoothly zoom in
                _targetFOV = weaponData.aimDownSightFOV;
                _targetWeaponFOV = weaponData.aimDownSightFOV;  // Set weapon FOV to be the same as player FOV
                playerLook.SetAimingDownSight(true);
            }
            else
            {
                // Reset the weapon to its original position
                adsPositionRef.localPosition = Vector3.Lerp(adsPositionRef.localPosition, _originalAdsLocalPosition, Time.deltaTime * weaponData.aimDownSightSpeed);

                // Smoothly zoom out
                _targetFOV = weaponData.originalPlayerFOV;
                _targetWeaponFOV = weaponData.originalWeaponFOV;
                playerLook.SetAimingDownSight(false);
            }

            // Set the player camera FOV
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, _targetFOV, Time.deltaTime * weaponData.zoomSpeed);

            // Set the weapon camera FOV
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, _targetWeaponFOV, Time.deltaTime * weaponData.zoomSpeed);
        }

        private void UpdateAiming(bool aiming)
        {
            _aimingDownSight = aiming;
        }

        private void HandleAdsInput()
        {
            if (_reloading)
            {
                return;
            }
            if (toggleAimDownSight)
            {
                if (Input.GetKeyDown(aimDownSightKey))
                    UpdateAiming(!_aimingDownSight);
            }
            else
            {
                UpdateAiming(Input.GetKey(aimDownSightKey));
            }
        }

        private void ResetFOV()
        {
            playerCamera.fieldOfView = weaponData.originalPlayerFOV;
            weaponCamera.fieldOfView = weaponData.originalWeaponFOV;
        }
        
        // ########
        // ########
        // ########
        // ########
        // ########
        // ########
        // ########
        
        //Separate these VFX things to another script!!!
        private void SpawnBloodParticle(Vector3 position)
        {
            if (_isbloodFXPrefabNull) return;
            GameObject bloodParticle = Instantiate(bloodFXPrefab, position, Quaternion.identity);
            Destroy(bloodParticle, 1f);
        }

        private void SpawnBulletHole(Vector3 position, Quaternion rotation)
        {
            if (_isbulletHolePrefabNull) return;
            GameObject bulletHole = Instantiate(bulletHolePrefab, position, rotation);
            Destroy(bulletHole, 5f);
        }

        private void OnGunShot()
        {
            
            if (_ismuzzleFlashPrefabNull) return;
            Vector3 muzzlePosition = muzzleTransform.position;
            Quaternion muzzleRotation = muzzleTransform.rotation;

            if (_aimingDownSight)
            {
                var fovRatio = weaponData.originalWeaponFOV / weaponCamera.fieldOfView;
                muzzlePosition += muzzleTransform.forward * fovRatio;
            }
            
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePosition, muzzleRotation);
            Destroy(muzzleFlash, 0.1f);
            switch (weaponType)
            {
                case WeaponType.Deagle:
                    audioMenager.PlayDeagleRanged();
                    break;
                case WeaponType.Rifle:
                    audioMenager.PlayRifleRanged();
                    break;
                case WeaponType.Shotgun:
                    audioMenager.PlayShotgunRanged();
                    break;
                default:
                    break;
            }
        }
        
        //Separate UI logic!!!
        private void UpdateAmmoUI()
        {
            if (_reloading)
            {
                currentAmmoText.text = "RELOADING";
            }
            else
            {
                currentAmmoText.text = "Ammo: " + weaponData.currentAmmo;
            }
        }
    }
}