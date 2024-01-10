using System.Collections;
using ECM.Components;
using ECM.Controllers;
using ECM.Examples;
using FMODUnity;
using HorrorEngine;
using UnityEngine;
using UnityEngine.UI;
using Interfaces;

namespace WeaponRelated
{
    public class Weapon : MonoBehaviour
    {
        public WeaponData weaponData;

        [Header("References")]
        private NewPlayerController _newPlayerController;
        private MouseLook _mouseLook;
        private Camera _playerCamera;
        private Camera _weaponCamera;
        private Transform _playerCameraTransform;
        private Transform _weaponsHolderTransform;
        private InventoryEntry _weaponEntry;
        private GameObject _crosshairInstance;
        // private float _duration;
        
        [Header("Transforms")]
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
        // private Text _currentAmmoText;
        
        [Header("Flags")]
        [SerializeField] private bool toggleAimDownSight = true;

        //Flags
        private bool _shooting;
        private bool _reloading;
        private Coroutine _reloadCoroutine;
        private bool _isbloodFXPrefabNull;
        private bool _isbulletHolePrefabNull;
        private bool _ismuzzleFlashPrefabNull;

        //Shooting & ADS variables
        [HideInInspector] public bool aimingDownSight;
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
            // _audioManager = GetComponent<AudioManager>();
            _newPlayerController = GetComponentInParent<NewPlayerController>();
            _mouseLook = GetComponentInParent<MouseLook>();
            _playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
            _weaponCamera = GameObject.Find("WeaponCamera").GetComponent<Camera>();
            _playerCameraTransform = GameObject.Find("Camera_Pivot").transform;
            _weaponsHolderTransform = GameObject.Find("WeaponsHolder").transform;
            
            _ismuzzleFlashPrefabNull = muzzleFlashPrefab == null;
            _isbulletHolePrefabNull = bulletHolePrefab == null;
            _isbloodFXPrefabNull = bloodFXPrefab == null;
            _timeSinceLastShot = weaponData.fireRate / 60f;
            _weaponOriginalLocalPosition = transform.localPosition;
            _originalAdsLocalPosition = adsPositionRef.localPosition;
            weaponData.originalPlayerFOV = _playerCamera.fieldOfView;
            weaponData.originalWeaponFOV = _weaponCamera.fieldOfView;
        }
        private void Start()
        {
            if (weaponData.crosshairPrefab != null)
            {
                _crosshairInstance = Instantiate(weaponData.crosshairPrefab, transform);
                _crosshairInstance.SetActive(false);
            }
            
            UpdateAiming(false);
        }

        private void OnDisable()
        {
            _shooting = false;
            _reloading = false;
            aimingDownSight = false;
            if (_crosshairInstance != null)
            {
                Destroy(_crosshairInstance);
            }
            ResetFOV();
        }

        private void Update()
        {
            if (PauseController.Instance.IsPaused)
            {
                return;
            }
            
            HandleReloadingInput();
            HandleAdsInput();
            HandleShootingInput();
            Aiming();
            // UpdateAmmoUI();
            UpdateCrosshairVisibility();
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
            return !_reloading && _timeSinceLastShot >= 1f / (weaponData.fireRate / 60f) && !_newPlayerController.run;
        }

        private void Shoot()
        {
            _weaponEntry = GameManager.Instance.Inventory.GetEquippedWeapon();

            if (_weaponEntry.SecondaryCount <= 0)
            {
                StartReload();
                return;
            }

            for (var i = 0; i < weaponData.bulletsPerShot; i++)
            {
                Vector3 spreadDirection = _weaponsHolderTransform.forward + Random.insideUnitSphere * weaponData.spread;
        
                var layerMask = ~LayerMask.GetMask("InteractionSystem");
                if (!Physics.Raycast(_weaponsHolderTransform.position, spreadDirection, out RaycastHit hitInfo, weaponData.maxDistance, layerMask)) continue;
        
                if (hitInfo.transform.CompareTag("Wall"))
                {
                    SpawnBulletHole(hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));
                }

                var damageable = hitInfo.transform.GetComponent<IDamageable>();
                if (damageable == null) continue;

                damageable.TakeDamage(weaponData.damage);
                SpawnBloodParticle(hitInfo.point);
                
                // if (hitInfo.transform.CompareTag("TarEnemyHeadshot"))
                // {
                //     damageable.TakeDamage(weaponData.damage + 10);
                // }
                //
                // if (hitInfo.transform.CompareTag("BossEnemyChipHit"))
                // {
                //     damageable.TakeDamage(weaponData.damage + 20);
                // }
            }

            if (_weaponEntry.SecondaryCount > 0)
                --_weaponEntry.SecondaryCount;
            _timeSinceLastShot = 0f;
    
            OnGunShot();
            CalculateRecoil();
            ApplyProceduralKickback();
        }

        private void CalculateRecoil()
        {
            Vector3 recoilRotation = aimingDownSight ? weaponData.recoilRotationAiming : weaponData.recoilRotationHipfire;

            var recoilMultiplier = 1f;
            if (_newPlayerController.IsWalking)
            {
                recoilMultiplier = weaponData.walkingRecoilMultiplier;
            }

            _smoothRotation += new Vector3
            (
                -recoilRotation.x,
                Random.Range(-recoilRotation.y, recoilRotation.y),
                Random.Range(-recoilRotation.z, recoilRotation.z)
            ) * recoilMultiplier;
        }
        
        private void ApplyRecoil()
        {
            Vector3 currentPlayerCameraRotation = Vector3.Lerp(_currentPlayerCameraRotation, _smoothRotation, weaponData.rotationSpeed * Time.deltaTime * weaponData.smoothingFactor);
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, weaponData.returnSpeed * Time.deltaTime * weaponData.smoothingFactor);
            _playerCameraTransform.localRotation = Quaternion.Euler(currentPlayerCameraRotation);
            
            Vector3 currentWeaponCameraRotation = Vector3.Lerp(_currentWeaponCameraRotation, _smoothRotation, weaponData.rotationSpeed * Time.deltaTime * weaponData.smoothingFactor);
            _smoothRotation = Vector3.Lerp(_smoothRotation, Vector3.zero, weaponData.returnSpeed * Time.deltaTime * weaponData.smoothingFactor);
            _weaponsHolderTransform.localRotation = Quaternion.Euler(currentWeaponCameraRotation);

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
            if (_reloading || !gameObject.activeSelf) return;

            _weaponEntry = GameManager.Instance.Inventory.GetEquippedWeapon();
            weaponData = _weaponEntry.Item as WeaponData;

            if (_weaponEntry.SecondaryCount < weaponData.MaxAmmo)
            {
                ReloadFromInventory();
            }
        }

        private IEnumerator Reload(int ammoToReload, InventoryEntry ammoEntry)
        {
            _reloading = true;
            aimingDownSight = false;
            UIManager.Get<UIInputListener>().AddBlockingContext(this);

            yield return new WaitForSeconds(weaponData.reloadTime);

            int actualAmmoLoaded = Mathf.Min(ammoToReload, ammoEntry.Count);
            _weaponEntry.SecondaryCount += actualAmmoLoaded;
            GameManager.Instance.Inventory.Remove(ammoEntry, actualAmmoLoaded);
            _weaponEntry.SecondaryCount = Mathf.Min(_weaponEntry.SecondaryCount, weaponData.MaxAmmo);

            EndReload();
        }
        
        private void ReloadFromInventory()
        {
            Inventory inventory = GameManager.Instance.Inventory;
            if (weaponData.AmmoItem != null && inventory.TryGet(weaponData.AmmoItem, out InventoryEntry ammoEntry))
            {
                int ammoNeeded = weaponData.MaxAmmo - _weaponEntry.SecondaryCount;
                int ammoAvailable = Mathf.Min(ammoEntry.Count, ammoNeeded);

                if (ammoAvailable > 0)
                {
                    if (_reloadCoroutine != null)
                    {
                        StopCoroutine(_reloadCoroutine);
                    }
                    _reloadCoroutine = StartCoroutine(Reload(ammoAvailable, ammoEntry));
                }
                else
                {
                    // Handle no ammo available scenario
                }
            }
            else
            {
                // Handle no ammo available scenario
            }
        }

        private void EndReload()
        {
            _reloading = false;
            UIManager.Get<UIInputListener>().RemoveBlockingContext(this);
            _reloadCoroutine = null;
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
            if (aimingDownSight)
            {
                var targetScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

                var distanceFromCamera = Vector3.Distance(adsPositionRef.position, _playerCamera.transform.position);
                Vector3 targetWorldPosition = _playerCamera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, distanceFromCamera));

                Vector3 targetLocalPosition = adsPositionRef.parent.InverseTransformPoint(targetWorldPosition);

                adsPositionRef.localPosition = Vector3.Lerp(adsPositionRef.localPosition, targetLocalPosition, Time.deltaTime * weaponData.aimDownSightSpeed);

                _targetFOV = weaponData.aimDownSightFOV;
                _targetWeaponFOV = weaponData.aimDownSightFOV;
                _mouseLook.SetAimingDownSight(true);
            }
            else
            {
                adsPositionRef.localPosition = Vector3.Lerp(adsPositionRef.localPosition, _originalAdsLocalPosition, Time.deltaTime * weaponData.aimDownSightSpeed);

                _targetFOV = weaponData.originalPlayerFOV;
                _targetWeaponFOV = weaponData.originalWeaponFOV;
                _mouseLook.SetAimingDownSight(false);
            }

            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _targetFOV, Time.deltaTime * weaponData.zoomSpeed);
            _weaponCamera.fieldOfView = Mathf.Lerp(_weaponCamera.fieldOfView, _targetWeaponFOV, Time.deltaTime * weaponData.zoomSpeed);
        }

        private void UpdateAiming(bool aiming)
        {
            aimingDownSight = aiming;
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
                    UpdateAiming(!aimingDownSight);
            }
            else
            {
                UpdateAiming(Input.GetKey(aimDownSightKey));
            }
        }

        private void ResetFOV()
        {
            _playerCamera.fieldOfView = weaponData.originalPlayerFOV;
            _weaponCamera.fieldOfView = weaponData.originalWeaponFOV;
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

            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleTransform);

            muzzleFlash.transform.localPosition = Vector3.zero;
            muzzleFlash.transform.localRotation = Quaternion.identity;

            if (aimingDownSight)
            {
                // Might need to fine-tune these values.
                muzzleFlash.transform.localPosition = new Vector3(0f, 0f, 0.1f);

                // Optionally can scale down the muzzle flash for ADS mode
                muzzleFlash.transform.localScale = Vector3.one * 0.5f;
            }

            Destroy(muzzleFlash, 0.1f);

            PlayGunShotSFX();
        }
        
        private void PlayGunShotSFX()
        {
            if (weaponData.gunShotSFX.IsNull)
            {
                Debug.LogWarning("Fmod event not found: gunShotSFX");
                return;
            }
            RuntimeManager.PlayOneShot(weaponData.gunShotSFX, transform.position);
        }
        
        //Separate UI logic!!!
        
        
        private void UpdateCrosshairVisibility()
        {
            if (_crosshairInstance != null)
            {
                _crosshairInstance.SetActive(!aimingDownSight);
            }
        }
        
        /*private void UpdateCrosshair()
        {
            // Determine crosshair size based on the player's state and weapon's characteristics
            if (_shooting)
            {
                _currentSize = Mathf.Lerp(_currentSize, weaponData.maxSize, weaponData.speed * Time.deltaTime);
            }
            else if (_newPlayerController.IsWalking)
            {
                _currentSize = Mathf.Lerp(_currentSize, weaponData.restingSize * weaponData.walkingRecoilMultiplier, weaponData.speed * Time.deltaTime);
            }
            else
            {
                _currentSize = Mathf.Lerp(_currentSize, weaponData.restingSize, weaponData.speed * Time.deltaTime);
            }

            ApplyCrosshairSize();
        }

        private void ApplyCrosshairSize()
        {
            _currentSize = Mathf.Lerp(_currentSize, weaponData.restingSize, Time.deltaTime * weaponData.speed);
            _reticle.sizeDelta = new Vector2(_currentSize, _currentSize);
            // Update the size of each crosshair part
            // float sizeDelta = _currentSize - weaponData.restingSize;
            // topCrosshair.sizeDelta = new Vector2(topCrosshair.sizeDelta.x, weaponData.restingSize + sizeDelta);
            // bottomCrosshair.sizeDelta = new Vector2(bottomCrosshair.sizeDelta.x, weaponData.restingSize + sizeDelta);
            // leftCrosshair.sizeDelta = new Vector2(leftCrosshair.sizeDelta.y, weaponData.restingSize + sizeDelta);
            // rightCrosshair.sizeDelta = new Vector2(rightCrosshair.sizeDelta.y, weaponData.restingSize + sizeDelta);
        }*/
        
        // private void UpdateAmmoUI()
        // {
        //     if (_reloading)
        //     {
        //         _currentAmmoText.text = "RELOADING";
        //     }
        //     else
        //     {
        //         _currentAmmoText.text = "Ammo: " + weaponData.currentAmmo;
        //     }
        // }
    }
}