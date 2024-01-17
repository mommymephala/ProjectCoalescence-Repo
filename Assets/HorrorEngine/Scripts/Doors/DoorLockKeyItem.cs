using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HorrorEngine
{
    public class DoorLockKeyItem : DoorLock
    {
        [SerializeField] private DialogData m_OnUnlockedDialog;
        [SerializeField] private AudioClip m_OnUnlockedSound;
        [SerializeField] private DialogData m_OnLockedOtherSideDialog;
        
        [Space]
        [SerializeField] private ItemData m_Key;
        [SerializeField] private bool m_ConsumesItem = true;
        // [SerializeField] private bool m_UseObjectAutomatically = true;
        // [SerializeField] private bool m_UseKeyBothSides = true;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(m_Key, "DoorLockKeyItem requires an item to work", gameObject);
        }

        public override void OnTryToUnlock(out bool openImmediately)
        {
            if (!TryUnlock())
            {
                if (m_LockedSound)
                    AudioSource.PlayOneShot(m_LockedSound);

                if (m_OnLockedDialog.IsValid())
                    UIManager.Get<UIDialog>().Show(m_OnLockedDialog);
            }

            openImmediately = false;
        }

        /*public override void OnTryToUnlockFromExit(out bool openImmediately)
        {
            openImmediately = false;

            if (!m_UseKeyBothSides)
            {
                if (m_LockedSound)
                    AudioSource.PlayOneShot(m_LockedSound);

                if (m_OnLockedOtherSideDialog.IsValid())
                    UIManager.Get<UIDialog>().Show(m_OnLockedOtherSideDialog);
                return;
            }

            if (!TryUnlock() && m_OnLockedDialog.IsValid())
                UIManager.Get<UIDialog>().Show(m_OnLockedDialog);
        }*/

        public bool TryUnlock()
        {
            Debug.Assert(m_Key, "DoorLockKeyItem requires an item to work", gameObject);
            if (!m_Key)
                return false;
            
            if (GameManager.Instance.Inventory.Contains(m_Key))
            {
                if (m_ConsumesItem)
                {
                    GameManager.Instance.Inventory.Remove(m_Key);
                    IsLocked = false;
                        
                    OnUnlock?.Invoke();
                        
                    if (m_OnUnlockedSound)
                        AudioSource.PlayOneShot(m_OnUnlockedSound);

                    if (m_OnUnlockedDialog.IsValid())
                        UIManager.Get<UIDialog>().Show(m_OnUnlockedDialog);
                }
                    
                return true;
            }

            return false;
        }
    }
}