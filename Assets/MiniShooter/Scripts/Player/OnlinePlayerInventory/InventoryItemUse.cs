using UnityEngine;

namespace MiniShooter
{
    public abstract class InventoryItemUse : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        protected InventoryItem inventoryItem;

        #endregion

        public InventoryItem Item => inventoryItem;

        public abstract bool Use(OnlinePlayer player);
    }
}
