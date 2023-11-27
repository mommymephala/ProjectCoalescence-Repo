using System.Collections.Generic;
using UnityEngine;
using WeaponRelated;

namespace Managers
{
    public class Inventory : MonoBehaviour
    {
        private const int MaxInventorySlots = 2;
        public List<WeaponData> weapons = new List<WeaponData>();
        
        public void AddItem(WeaponData weapon) // Update parameter type
        {
            if (weapons.Count >= MaxInventorySlots || HasItem(weapon)) return;
            weapons.Add(weapon);
            DebugList();
        }

        public void RemoveItem(WeaponData weapon) // Update parameter type
        {
            if (weapons.Count <= 0 || !HasItem(weapon)) return;
            weapons.Remove(weapon);
            DebugList();
        }

        private void DebugList()
        {
            if (weapons.Count == 0)
                Debug.Log("List is empty.");
            foreach (WeaponData weapon in weapons) // Update iteration type
            {
                Debug.Log(weapon);
            }
        }

        private bool HasItem(WeaponData weapon) // Update parameter type
        {
            return weapons.Contains(weapon);
        }
    }
}