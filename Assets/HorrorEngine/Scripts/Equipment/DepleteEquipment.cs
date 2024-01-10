using UnityEngine;
using UnityEngine.Events;

namespace HorrorEngine
{
    public class DepleteEquipment : MonoBehaviour
    {
        [SerializeField] private EquipableItemData m_Item;
        [SerializeField] private float m_Depletion = 0.01f;

        public UnityEvent<float> OnDepleted;

        private void Awake()
        {
            Debug.Assert(m_Depletion > 0, "Equipment m_Depletion amount can't be less than 0", gameObject);
        }
        
        public bool HasCharge()
        {
            var equipped = GameManager.Instance.Inventory.GetEquipped(m_Item.Slot);
            return equipped.Status > 0; // Assuming charge is represented by the Status value
        }

        public void Deplete()
        {
            var equipped = GameManager.Instance.Inventory.GetEquipped(m_Item.Slot);
            Debug.Assert(equipped.Item == m_Item, "Item to deplete does not match equipped item");

            float prevStatus = equipped.Status;
            equipped.Status = Mathf.Clamp01(equipped.Status - m_Depletion);
            
            if (equipped.Status != prevStatus)
                OnDepleted.Invoke(equipped.Status);
        }
    }
}