using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEngine
{
    [CreateAssetMenu(menuName = "Horror Engine/Items/Reloadable Weapon")]
    public class ReloadableHEWeaponData : HEWeaponData
    {
        public ItemData AmmoItem;
        public int MaxAmmo;
        // public AudioClip ShotSound;
        // public AudioClip ReloadSound;
        // public AudioClip NoAmmoSound;
    }
}