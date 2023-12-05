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

            // Check for key presses and change the selected weapon if enough time has passed since the last switch
            for (var i = 0; i < keys.Length; i++)
                if (Input.GetKeyDown(keys[i]) && _timeSinceLastSwitch >= switchTime)
                    _selectedWeapon = i;

            if (previousSelectedWeapon != _selectedWeapon)
                Select(_selectedWeapon);

            _timeSinceLastSwitch += Time.deltaTime;
        }
        
        private void SetWeaponSlots() 
        {
            // Gather all child weapon slots and store them in the _weaponSlots array
            _weaponSlots = new GameObject[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
                _weaponSlots[i] = transform.GetChild(i).gameObject;

            // If keys array is null, initialize it to the length of _weaponSlots
            keys ??= new KeyCode[_weaponSlots.Length];
        }

        private void Select(int weaponIndex)
        {
            // Activates the selected weapon and deactivates others
            for (var i = 0; i < _weaponSlots.Length; i++)
                _weaponSlots[i].SetActive(i == weaponIndex);

            _timeSinceLastSwitch = 0f;

            //OnWeaponSelected();
        }

        //Fill this method later.
        //private void OnWeaponSelected() { }
    }
}
