using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Interfaces;
using PlayerActions;

namespace WeaponRelated
{
    public class Weapon : MonoBehaviour
    {
        [Header("References")]
        public WeaponData weaponData;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private Transform playerCameraTransform;
        
        //Separate visual logic
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private GameObject bloodFXPrefab;
        [SerializeField] private GameObject bulletHolePrefab;
        //Separate visual logic
        
        [SerializeField] private Camera playerCamera;
        [SerializeField] private KeyCode aimDownSightKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode reloadKey = KeyCode.R;
        [SerializeField] private bool toggleAimDownSight = true;
        [SerializeField] private Transform adsPositionRef;
        
        //Separate UI logic
        [Header("UI")]
        [SerializeField] private Text currentAmmoText;

        //Flags
        private bool _shooting;
        private bool _reloading;
        
        //Separate visual logic
        private bool _isbloodFXPrefabNull;
        private bool _isbulletHolePrefabNull;
        private bool _ismuzzleFlashPrefabNull;

        //Shooting & ADS variables
        private bool _aimingDownSight;
        private Vector3 _originalWeaponPosition;
        private float _timeSinceLastShot;
        private float _targetFOV;
    
        // Properties to access recoil-related variables
        private Vector3 _currentCameraRotation;
        private Vector3 _smoothRotation;
        private Vector3 _rotationalRecoil;
        private Vector3 _positionalRecoil;
        private Vector3 _weaponOriginalLocalPosition;
        private Quaternion _originalWeaponRotationAds;

        private void Awake()
        {
            _ismuzzleFlashPrefabNull = muzzleFlashPrefab == null;
            _isbulletHolePrefabNull = bulletHolePrefab == null;
            _isbloodFXPrefabNull = bloodFXPrefab == null;
            _timeSinceLastShot = weaponData.fireRate / 60f;
            _weaponOriginalLocalPosition = transform.localPosition;
            _originalWeaponPosition = adsPositionRef.localPosition;
            _originalWeaponRotationAds = adsPositionRef.localRotation;
            weaponData.originalFOV = playerCamera.fieldOfView;
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

        private void FixedUpdate()
        {
            ApplyRecoilReset();
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
            return !_reloading && _timeSinceLastShot >= 1f / (weaponData.fireRate / 60f);
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
                Vector3 spreadDirection = playerCameraTransform.forward + Random.insideUnitSphere * weaponData.spread;
                if (!Physics.Raycast(playerCameraTransform.position, spreadDirection, out RaycastHit hitInfo,
                        weaponData.maxDistance)) continue;
                Debug.DrawRay(playerCameraTransform.position, spreadDirection, Color.red, 5);
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
            ApplyRecoil();
            
            //Re-write
            StartCoroutine(ApplyKickback());
            StartCoroutine(ResetWeaponPositionAfterKickback());
            StartCoroutine(ResetWeaponRotationAfterKickback());
        }

        private void ApplyRecoil()
        {
            Vector3 recoilRotation = _aimingDownSight ? weaponData.recoilRotationAiming : weaponData.recoilRotationHipfire;

            // Check if the player is sprinting or walking and apply the appropriate recoil multiplier
            var recoilMultiplier = 1f;
            if (playerMovement.IsSprinting)
            {
                recoilMultiplier = weaponData.sprintingRecoilMultiplier;
            }
            else if (playerMovement.IsWalking)
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
        
        private void ApplyRecoilReset()
        {
            // Gradually smooth the recoil rotation back to zero
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, weaponData.returnSpeed * Time.deltaTime);

            // Apply the current camera rotation (including the smoothed recoil) to the camera
            _currentCameraRotation = Vector3.Slerp(_currentCameraRotation, _rotationalRecoil + _smoothRotation, weaponData.rotationSpeed * Time.fixedDeltaTime);
            playerCameraTransform.localRotation = Quaternion.Euler(_currentCameraRotation);
        }
        
        //Re-write or animate

        private IEnumerator ApplyKickback()
        {
            // Apply the kickback force in the opposite direction of the weapon's forward vector
            Vector3 kickbackDirection = -transform.forward;
            var elapsedTime = 0f;
            Vector3 initialPosition = transform.localPosition;
            Quaternion initialRotation = transform.localRotation;

            while (elapsedTime < weaponData.kickbackDuration)
            {
                // Calculate the kickback amount using Lerp for smoothness
                var kickbackAmount = Mathf.Lerp(0f, weaponData.kickbackForce, elapsedTime / weaponData.kickbackDuration);

                // Apply the positional kickback only in the backward direction
                Vector3 kickbackVector = kickbackDirection * kickbackAmount;

                // Update the weapon's local position with the positional kickback
                transform.localPosition = initialPosition + kickbackVector;

                // Calculate the rotational kickback amount using Lerp for smoothness
                var rotationalKickbackAmount = Mathf.Lerp(0f, weaponData.kickbackRotation, elapsedTime / weaponData.kickbackDuration);

                // Generate a random rotation for the rotational kickback
                var randomRotation = new Vector3(Random.Range(-rotationalKickbackAmount, rotationalKickbackAmount), Random.Range(-rotationalKickbackAmount, rotationalKickbackAmount), 0f);

                // Apply the rotational kickback
                transform.localRotation = initialRotation * Quaternion.Euler(randomRotation);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the weapon is in its exact original position and rotation
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        }

        private IEnumerator ResetWeaponPositionAfterKickback()
        {
            // Wait for the kickback duration
            yield return new WaitForSeconds(weaponData.kickbackDuration);

            // Smoothly return the weapon to its original position
            var elapsedTime = 0f;
            Vector3 startPosition = transform.localPosition;
            while (elapsedTime < weaponData.resetDuration)
            {
                transform.localPosition = Vector3.Lerp(startPosition, _weaponOriginalLocalPosition, elapsedTime / weaponData.resetDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the weapon is in its exact original position
            transform.localPosition = _weaponOriginalLocalPosition;
        }
        
        private IEnumerator ResetWeaponRotationAfterKickback()
        {
            // Wait for the kickback duration
            yield return new WaitForSeconds(weaponData.kickbackDuration);

            // Smoothly return the weapon to its original rotation for ADS mode
            var elapsedTime = 0f;
            Quaternion startRotation = adsPositionRef.localRotation;
            while (elapsedTime < weaponData.resetDuration)
            {
                adsPositionRef.localRotation = Quaternion.Lerp(startRotation, _originalWeaponRotationAds, elapsedTime / weaponData.resetDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the weapon is in its exact original rotation for ADS mode
            adsPositionRef.localRotation = _originalWeaponRotationAds;
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

                // Apply the specified offsets
                targetLocalPosition += new Vector3(weaponData.adsOffsetX, weaponData.adsOffsetY, weaponData.adsOffsetZ);
                targetLocalPosition.z = weaponData.adsOffsetZ;

                // Lerp the weapon position to match the target position
                adsPositionRef.localPosition = Vector3.Lerp(adsPositionRef.localPosition, targetLocalPosition, Time.deltaTime * weaponData.aimDownSightSpeed);

                // Smoothly zoom in
                _targetFOV = weaponData.aimDownSightFOV;
                playerLook.SetAimingDownSight(true); // Call SetAimingDownSight with true to indicate aiming
            }
            else
            {
                // Reset the weapon to its original position
                adsPositionRef.localPosition = Vector3.Lerp(adsPositionRef.localPosition, _originalWeaponPosition, Time.deltaTime * weaponData.aimDownSightSpeed);

                // Smoothly zoom out
                _targetFOV = weaponData.originalFOV;
                playerLook.SetAimingDownSight(false); // Call SetAimingDownSight with false to indicate not aiming
            }

            // Lerp the playerCamera's field of view to the target FOV smoothly
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, _targetFOV, Time.deltaTime * weaponData.zoomSpeed);
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
            playerCamera.fieldOfView = weaponData.originalFOV;
        }
        
        //Separate these VFX things to another script.

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
                var fovRatio = weaponData.originalFOV / playerCamera.fieldOfView;
                muzzlePosition += muzzleTransform.forward * fovRatio;
            }

            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzlePosition, muzzleRotation);
            Destroy(muzzleFlash, 0.1f);
        }
        
        //Separate UI logic.

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