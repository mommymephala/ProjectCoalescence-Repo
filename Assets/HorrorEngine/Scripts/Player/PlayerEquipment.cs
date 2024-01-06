using System.Collections.Generic;
using UnityEngine;
using WeaponRelated;

namespace HorrorEngine
{
    public class PlayerEquipment : MonoBehaviour, IResetable
    {
        private struct EquipmentEntry
        {
            public GameObject Instance;
            public ItemData Data;
        }

        // --------------------------------------------------------------------

        private Dictionary<EquipmentSlot, EquipmentEntry> m_CurrentEquipment = new Dictionary<EquipmentSlot, EquipmentEntry>();
        
        [Header("Key Codes")]
        [SerializeField] private KeyCode holsterKey = KeyCode.H; // Key to holster the weapon

        [SerializeField] private GameObject weaponsHolder;
        // private SocketController m_SocketController;

        // --------------------------------------------------------------------

        private void Awake()
        {
            // m_SocketController = GetComponentInChildren<SocketController>();
            MessageBuffer<EquippedItemChangedMessage>.Subscribe(OnEquippedItemChanged);
        }

        // --------------------------------------------------------------------

        private void Start()
        {
            SetupCurrentEquipment();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryActivateEquipment(EquipmentSlot.Primary);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryActivateEquipment(EquipmentSlot.Secondary);
            }
            else if (Input.GetKeyDown(holsterKey))
            {
                HolsterEquipment();
            }
        }
        
        private void TryActivateEquipment(EquipmentSlot slot)
        {
            if (IsEquipmentSlotFilled(slot))
            {
                ActivateEquipment(slot);
            }
        }
        
        private void SetupCurrentEquipment()
        {
            Dictionary<EquipmentSlot, InventoryEntry> equipped = GameManager.Instance.Inventory.Equipped;
            foreach (var e in equipped)
            {
                var equipable = e.Value.Item as EquipableItemData;
                if (equipable.AttachOnEquipped)
                    Equip(equipable, equipable.Slot);
            }
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            MessageBuffer<EquippedItemChangedMessage>.Unsubscribe(OnEquippedItemChanged);
        }

        // --------------------------------------------------------------------

        private void OnEquippedItemChanged(EquippedItemChangedMessage msg)
        {
            if (msg.InventoryEntry != null)
            {
                EquipableItemData equipable = msg.InventoryEntry.Item as EquipableItemData;
                if (equipable.AttachOnEquipped)
                    Equip(equipable, equipable.Slot);
            }
            else
            {
                Unequip(msg.Slot);
            }
        }

        // --------------------------------------------------------------------

        public void Equip(EquipableItemData equipable, EquipmentSlot slot)
        {
            if (m_CurrentEquipment.ContainsKey(slot))
                Unequip(slot);

            GameObject instance = Instantiate(equipable.EquipPrefab, weaponsHolder.transform);
            
            m_CurrentEquipment.Add(slot, new EquipmentEntry()
            {
                Instance = instance,
                Data = equipable
            });

            // Activate only if it's the primary weapon
            if (slot == EquipmentSlot.Primary)
            {
                instance.SetActive(true);
                DeactivateOtherWeapons(EquipmentSlot.Secondary);
            }
            else
            {
                instance.SetActive(false);
            }
            
            // m_SocketController.Attach(instance, equipable.CharacterAttachment);
        }
        
        private void DeactivateOtherWeapons(EquipmentSlot slotToDeactivate)
        {
            if (m_CurrentEquipment.TryGetValue(slotToDeactivate, out EquipmentEntry entry))
            {
                entry.Instance.SetActive(false);
            }
        }
        
        private bool IsEquipmentSlotFilled(EquipmentSlot slot)
        {
            return m_CurrentEquipment.ContainsKey(slot) && m_CurrentEquipment[slot].Instance != null;
        }
        
        private void ActivateEquipment(EquipmentSlot slot)
        {
            foreach (var equipment in m_CurrentEquipment)
            {
                equipment.Value.Instance.SetActive(equipment.Key == slot);
            }
        }
    
        // --------------------------------------------------------------------

        private void Unequip(EquipmentSlot type, bool destroy = true)
        {
            if (m_CurrentEquipment.TryGetValue(type, out EquipmentEntry entry))
            {
                if (destroy && Application.isPlaying)
                    Destroy(entry.Instance);

                m_CurrentEquipment.Remove(type);
            }
        }
        
        private void HolsterEquipment()
        {
            foreach (var equipment in m_CurrentEquipment)
            {
                equipment.Value.Instance.SetActive(false);
            }
        }

        // --------------------------------------------------------------------

        /*public bool GetEquipped(EquipmentSlot type, out ItemData item, out GameObject instance)
        {
            item = null;
            instance = null;

            if (m_CurrentEquipment.TryGetValue(type, out EquipmentEntry entry))
            {
                item = entry.Data;
                instance = entry.Instance;
                return true;
            }

            return false;
        }*/

        // --------------------------------------------------------------------

        // public GameObject GetWeaponInstance(EquipmentSlot type)
        // {
        //     if (m_CurrentEquipment.TryGetValue(type, out EquipmentEntry entry))
        //     {
        //         if (entry.Data as WeaponData)
        //             return entry.Instance;
        //     }
        //     return null;
        // }

        // --------------------------------------------------------------------

        public void OnReset()
        {
            RemoveAllEquipment();
            SetupCurrentEquipment();
        }

        // --------------------------------------------------------------------

        private void RemoveAllEquipment()
        {
            foreach (var e in m_CurrentEquipment)
            {
                Destroy(e.Value.Instance);
            }
            m_CurrentEquipment.Clear();
        }
    }
}
