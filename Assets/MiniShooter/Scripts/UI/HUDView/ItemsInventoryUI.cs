using MasterServerToolkit.Extensions;
using MasterServerToolkit.MasterServer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniShooter
{
    public class ItemsInventoryUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private RectTransform container;
        [SerializeField]
        private InventoryItemUI inventoryItemUIPrefab;

        #endregion

        private readonly Dictionary<string, InventoryItemUI> inventoryItemUIs = new Dictionary<string, InventoryItemUI>();

        private void Awake()
        {
            container.RemoveChildren();

            OnlinePlayerCharacter.OnLocalCharacterCreatedEvent += OnlinePlayerCharacter_OnLocalCharacterCreatedEvent;
            OnlinePlayerCharacter.OnLocalCharacterDestroyedEvent += OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent;
        }

        private void OnDestroy()
        {
            OnlinePlayerCharacter.OnLocalCharacterCreatedEvent -= OnlinePlayerCharacter_OnLocalCharacterCreatedEvent;
            OnlinePlayerCharacter.OnLocalCharacterDestroyedEvent -= OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent;
        }

        private void OnlinePlayerCharacter_OnLocalCharacterCreatedEvent(PlayerCharacter playerCharacter)
        {
            if (OnlinePlayer.Local.Profile.Current != null)
                OnProfileLoaded();

            OnlinePlayer.Local.Profile.OnProfileLoadedEvent.AddListener(OnProfileLoaded);
        }

        private void OnProfileLoaded()
        {
            if (OnlinePlayer.Local.Profile.Current.TryGet(ProfilePropertyKeys.items, out ObservableDictStringInt items))
            {
                DrawItemUIs(items);
            }

            OnlinePlayer.Local.Profile.Current.OnPropertyUpdatedEvent += Profile_OnPropertyUpdatedEvent;
        }

        private void Profile_OnPropertyUpdatedEvent(ushort propertyCode, IObservableProperty property)
        {
            if (propertyCode == ProfilePropertyKeys.items)
                DrawItemUIs(property.As<ObservableDictStringInt>());
        }

        private void DrawItemUIs(ObservableDictStringInt items)
        {
            if (OnlinePlayer.Local != null)
            {
                foreach (var itemUI in inventoryItemUIs.Values.Where(i => i))
                    itemUI.gameObject.SetActive(false);

                foreach (var kvp in items)
                {
                    InventoryItemUI itemUI;

                    if (!inventoryItemUIs.ContainsKey(kvp.Key))
                    {
                        itemUI = Instantiate(inventoryItemUIPrefab, container, false);

                        if (OnlinePlayer.Local.Inventory.ItemsDatabase.TryGetItem(kvp.Key, out InventoryItem item))
                            itemUI.SetIcon(item.ItemIcon);

                        if (OnlinePlayer.Local.Inventory.TryGetItemUseKeyByItemId(kvp.Key, out InventoryItemUseKey itemUseKey))
                        {
                            if (!string.IsNullOrEmpty(itemUseKey.keyCodeName))
                                itemUI.SetKeyCode(itemUseKey.keyCodeName);
                            else
                                itemUI.SetKeyCode(itemUseKey.keyCode.ToString());
                        }

                        inventoryItemUIs[kvp.Key] = itemUI;
                    }

                    if (inventoryItemUIs.TryGetValue(kvp.Key, out itemUI) && itemUI)
                    {
                        itemUI.SetQuantity(kvp.Value);
                        itemUI.gameObject.SetActive(kvp.Value > 0);
                    }
                }
            }
        }

        private void OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent()
        {
            container.RemoveChildren();
            inventoryItemUIs.Clear();
            OnlinePlayer.Local.Profile.Current.OnPropertyUpdatedEvent -= Profile_OnPropertyUpdatedEvent;
            OnlinePlayer.Local.Profile.OnProfileLoadedEvent.RemoveListener(OnProfileLoaded);
        }
    }
}