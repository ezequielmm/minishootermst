using MasterServerToolkit.MasterServer;
using System.Collections.Generic;
using UnityEngine;

namespace MiniShooter
{
    [CreateAssetMenu(menuName = MstConstants.CreateMenu + "Mini shooter/InventoryItemsDatabase")]
    public class InventoryItemsDatabase : ScriptableObject
    {
        [SerializeField]
        private InventoryItem[] items;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public InventoryItem GetItem(string itemId)
        {
            for (int i = 0; i < items.Length; i++)
            {
                InventoryItem item = items[i];

                if (item.ItemId == itemId)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public InventoryItemUse GetUseableItem(string itemId)
        {
            if (TryGetItem(itemId, out InventoryItem item) && item.TryGetComponent(out InventoryItemUse useableItem))
            {
                return useableItem;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetItem(string itemId, out InventoryItem item)
        {
            item = GetItem(itemId);
            return item != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TyrGetUseableItem(string itemId, out InventoryItemUse item)
        {
            item = GetUseableItem(itemId);
            return item != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerator<InventoryItem> GetEnumerator()
        {
            foreach (var item in items)
                yield return item;
        }
    }
}
