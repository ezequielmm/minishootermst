using Mirror;
using UnityEngine;

namespace MiniShooter
{
    [RequireComponent(typeof(InventoryItemRangeTrigger))]
    public class InventoryItem : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Base Info"), SerializeField]
        private string itemId = "itemName";
        [SerializeField]
        private string itemTitle = "itemTitle";
        [SerializeField]
        private Sprite itemIcon;
        [SerializeField]
        private int maxQuantity = 1;
        [SerializeField]
        private InventoryItemType type;

        [Header("Store Info"), SerializeField]
        private int buyPrice = 1;

        #endregion

        private int quantity = 1;

        public string ItemId => itemId;
        public string ItemTitle => itemTitle;
        public Sprite ItemIcon => itemIcon;
        public InventoryItemType ItemType => type;
        public int Quantity => quantity;
        public int MaxQuantity => maxQuantity;
        public int BuyPrice => buyPrice;

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(itemId))
                itemId = name;

            if (string.IsNullOrEmpty(itemTitle))
                itemTitle = itemId;
        }

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();

            quantity = Random.Range(1, maxQuantity + 1);
        }

        [Server]
        public void PickUp()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion
    }
}