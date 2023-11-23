using System;

namespace MiniShooter
{
    [Serializable]
    public struct StartupWeaponItemInfo
    {
        public InventoryItem item;
        public int currentAmmo;
        public int totalAmmo;
    }
}