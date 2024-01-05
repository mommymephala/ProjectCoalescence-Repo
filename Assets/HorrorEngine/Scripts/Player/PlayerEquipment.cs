using System.Collections.Generic;
using UnityEngine;
using WeaponRelated;

namespace HorrorEngine
{
    public class PlayerEquipment : MonoBehaviour, IResetable
    {
        public struct EquipmentEntry
        {
            public GameObject Instance;
            public ItemData Data;
        }

        // --------------------------------------------------------------------

        private Dictionary<EquipmentSlot, EquipmentEntry> m_CurrentEquipment = new Dictionary<EquipmentSlot, EquipmentEntry>();

        public GameObject weaponsHolder;
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
                ActivateEquipment(EquipmentSlot.Primary);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivateEquipment(EquipmentSlot.Secondary);
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

        public GameObject Equip(EquipableItemData equipable, EquipmentSlot slot)
        {
            if (m_CurrentEquipment.ContainsKey(slot))
                Unequip(slot);

            var instance = Instantiate(equipable.EquipPrefab, weaponsHolder.transform);
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

            return instance;
        }
        
        private void DeactivateOtherWeapons(EquipmentSlot slotToDeactivate)
        {
            if (m_CurrentEquipment.TryGetValue(slotToDeactivate, out EquipmentEntry entry))
            {
                entry.Instance.SetActive(false);
            }
        }
        private void ActivateEquipment(EquipmentSlot slot)
        {
            foreach (var equipment in m_CurrentEquipment)
            {
                if (equipment.Key == slot)
                {
                    equipment.Value.Instance.SetActive(true);
                }
                else
                {
                    equipment.Value.Instance.SetActive(false);
                }
            }
        }
        // --------------------------------------------------------------------

        public void Unequip(EquipmentSlot type, bool destroy = true)
        {
            if (m_CurrentEquipment.TryGetValue(type, out EquipmentEntry entry))
            {
                if (destroy && Application.isPlaying)
                    Destroy(entry.Instance);

                m_CurrentEquipment.Remove(type);
            }
        }

        // --------------------------------------------------------------------

        public bool GetEquipped(EquipmentSlot type, out ItemData item, out GameObject instance)
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
        }

        // --------------------------------------------------------------------

        public GameObject GetWeaponInstance(EquipmentSlot type)
        {
            if (m_CurrentEquipment.TryGetValue(type, out EquipmentEntry entry))
            {
                if (entry.Data as WeaponData)
                    return entry.Instance;
            }
            return null;
        }

        // --------------------------------------------------------------------

        public void OnReset()
        {
            RemoveAllEquipment();
            SetupCurrentEquipment();
        }

        // --------------------------------------------------------------------

        void RemoveAllEquipment()
        {
            foreach (var e in m_CurrentEquipment)
            {
                Destroy(e.Value.Instance);
            }
            m_CurrentEquipment.Clear();
        }
    }
}
