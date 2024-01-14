using UnityEngine;
using UnityEngine.Serialization;
using FMODUnity;
namespace HorrorEngine
{
    [CreateAssetMenu(menuName = "Horror Engine/Items/Equipable")]
    public class EquipableItemData : ItemData
    {
        public GameObject EquipPrefab;
        public EquipmentSlot Slot = EquipmentSlot.Primary;
        public bool AttachOnEquipped = true;
        public bool MoveOutOfInventoryOnEquip;

        public override void OnUse(InventoryEntry entry)
        {
            base.OnUse(entry);

            var equipment = GameManager.Instance.Player.GetComponent<PlayerEquipment>();
            Inventory inventory = GameManager.Instance.Inventory;
            if (entry != null)
            {
                EquipmentSlot slot = inventory.GetOccupyingEquipmentSlot(entry);
                if (slot != EquipmentSlot.None)
                    inventory.Unequip(Slot);
                else
                    inventory.Equip(entry);
            }
            else
            {
                equipment.Equip(this, this.Slot);
            }
        }
    }
}