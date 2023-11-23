using MasterServerToolkit.MasterServer;
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniShooter
{
    [RequireComponent(typeof(OnlinePlayer))]
    public class OnlinePlayerInventory : OnlinePlayerBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        protected InventoryItemUseKey[] itemUseKeys;

        [Header("Components"), SerializeField]
        protected InventoryItemsDatabase inventoryItemsDatabase;

        /// <summary>
        /// 
        /// </summary>
        public UnityEvent OnBoughtEvent;

        #endregion

        /// <summary>
        /// The current online player who owns this inventory
        /// </summary>
        protected OnlinePlayer onlinePlayer;

        public InventoryItemsDatabase ItemsDatabase => inventoryItemsDatabase;

        #region SHARED

        protected override void Awake()
        {
            base.Awake();
            onlinePlayer = GetComponent<OnlinePlayer>();
        }

        protected void Update()
        {
            if (isLocalPlayer)
            {
                foreach (var key in itemUseKeys)
                {
                    if (Input.GetKeyDown(key.keyCode))
                        UseItem(key.item.ItemId);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public InventoryItemUseKey GetItemUseKeyByItemId(string itemId)
        {
            return itemUseKeys.ToList().Find(i => i.item.ItemId == itemId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemUseKey"></param>
        /// <returns></returns>
        public bool TryGetItemUseKeyByItemId(string itemId, out InventoryItemUseKey itemUseKey)
        {
            itemUseKey = GetItemUseKeyByItemId(itemId);
            return itemUseKey.keyCode != KeyCode.None;
        }

        #endregion

        #region SERVER

        public override void OnServerPlayerReady()
        {
            base.OnServerPlayerReady();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        [Server]
        public void PickupItem(InventoryItem item)
        {
            IncreaseItemQuantity(item, item.Quantity);
        }

        /// <summary>
        /// Increases given item in inventory by <paramref name="quantity"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        private void IncreaseItemQuantity(InventoryItem item, int quantity)
        {
            if (item.ItemType == InventoryItemType.Item && Player.RoomPlayer.Profile.TryGet(ProfilePropertyKeys.items, out ObservableDictStringInt items))
            {
                if (!items.ContainsKey(item.ItemId))
                    items[item.ItemId] = 0;

                items[item.ItemId] += quantity;
            }
            else if (item.ItemType == InventoryItemType.Currency && Player.RoomPlayer.Profile.TryGet(ProfilePropertyKeys.money, out ObservableInt money))
            {
                money.Add(quantity);
            }
            else if (item.ItemType == InventoryItemType.Weapon && Player.RoomPlayer.Profile.TryGet(ProfilePropertyKeys.weapons, out ObservableWeapons weapons))
            {
                var weapon = weapons[item.ItemId];
                weapon.CurrentAmmo = weapon.CurrentAmmo;
                weapon.TotalAmmo += quantity;
                weapons[item.ItemId] = weapon;
            }
        }

        [Command]
        private void Cmd_UseItem(string itemId)
        {
            if (inventoryItemsDatabase.TyrGetUseableItem(itemId, out InventoryItemUse usuableItem)
                && Player.RoomPlayer.Profile.TryGet(ProfilePropertyKeys.items, out ObservableDictStringInt items)
                && items.ContainsKey(itemId)
                && items[itemId] > 0
                && usuableItem.Use(onlinePlayer))
            {
                int currentQuantity = items[itemId];

                if (currentQuantity > 0)
                {
                    currentQuantity--;
                    items[itemId] = currentQuantity;
                }

                onlinePlayer.NotificationRoomModule.NoticeToClient(connectionToClient.connectionId, $"Item \"{usuableItem.Item.ItemTitle}\" is applied");
            }
        }

        [Command]
        private void Cmd_BuyItem(string itemId)
        {
            if (inventoryItemsDatabase.TryGetItem(itemId, out InventoryItem item)
                && Player.RoomPlayer.Profile.TryGet(ProfilePropertyKeys.money, out ObservableInt money))
            {
                if (money.Subtract(item.BuyPrice, 0))
                {
                    IncreaseItemQuantity(item, item.MaxQuantity);
                    Rpc_OnBoughtEvent(connectionToClient);
                    onlinePlayer.NotificationRoomModule.NoticeToClient(connectionToClient.connectionId, $"You have just boght {item.ItemTitle}");
                }
                else
                {
                    onlinePlayer.NotificationRoomModule.NoticeToClient(connectionToClient.connectionId, "Not enough money");
                }
            }
        }

        #endregion

        #region CLIENT

        public override void OnLocalPlayerReady()
        {
            base.OnLocalPlayerReady();
        }

        [TargetRpc]
        private void Rpc_OnBoughtEvent(NetworkConnection con)
        {
            OnBoughtEvent?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        [Client]
        public void UseItem(string itemId)
        {
            Cmd_UseItem(itemId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        [Client]
        public void BuyItem(string itemId)
        {
            Cmd_BuyItem(itemId);
        }

        #endregion
    }
}