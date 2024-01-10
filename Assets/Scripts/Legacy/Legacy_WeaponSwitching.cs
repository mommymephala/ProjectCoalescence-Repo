using UnityEngine;

namespace WeaponRelated
{
    public class WeaponSwitching : MonoBehaviour
    { 
        [Header("Keys")]
        [SerializeField] private KeyCode[] keys;

        [Header("Settings")]
        [SerializeField] private float switchTime;
        
        private GameObject[] _weaponSlots;
        private int _selectedWeapon;
        private float _timeSinceLastSwitch;

        private void Start() 
        {
            SetWeaponSlots();
            Select(_selectedWeapon);
            _timeSinceLastSwitch = 0f;
        }
    
        private void Update() 
        {
            var previousSelectedWeapon = _selectedWeapon;

            for (var i = 0; i < keys.Length; i++)
                if (Input.GetKeyDown(keys[i]) && _timeSinceLastSwitch >= switchTime)
                    _selectedWeapon = i;

            if (previousSelectedWeapon != _selectedWeapon)
                Select(_selectedWeapon);

            _timeSinceLastSwitch += Time.deltaTime;
        }
        
        private void SetWeaponSlots() 
        {
            _weaponSlots = new GameObject[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
                _weaponSlots[i] = transform.GetChild(i).gameObject;

            keys ??= new KeyCode[_weaponSlots.Length];
        }

        private void Select(int weaponIndex)
        {
            for (var i = 0; i < _weaponSlots.Length; i++)
                _weaponSlots[i].SetActive(i == weaponIndex);

            _timeSinceLastSwitch = 0f;

            //OnWeaponSelected();
        }

        //Fill this method later.
        //private void OnWeaponSelected() { }
    }
}
