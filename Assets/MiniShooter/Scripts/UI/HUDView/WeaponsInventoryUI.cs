using MasterServerToolkit.Extensions;
using MasterServerToolkit.MasterServer;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniShooter
{
    public class WeaponsInventoryUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private RectTransform container;
        [SerializeField]
        private InventoryItemUI inventoryItemUIPrefab;

        #endregion

        private readonly Dictionary<string, InventoryItemUI> inventoryWeaponUIs = new Dictionary<string, InventoryItemUI>();

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
            if (OnlinePlayer.Local.Profile.Current.TryGet(ProfilePropertyKeys.weapons, out ObservableWeapons weapons))
            {
                DrawItemUIs(weapons);
            }

            OnlinePlayer.Local.Profile.Current.OnPropertyUpdatedEvent += Profile_OnPropertyUpdatedEvent;
        }

        private void Profile_OnPropertyUpdatedEvent(ushort propertyCode, IObservableProperty property)
        {
            if (propertyCode == ProfilePropertyKeys.weapons)
                DrawItemUIs(property.As<ObservableWeapons>());
        }

        private void DrawItemUIs(ObservableWeapons weapons)
        {
            if (OnlinePlayer.Local != null)
            {
                foreach (var itemUI in inventoryWeaponUIs.Values.Where(i => i))
                    itemUI.gameObject.SetActive(false);

                int useKey = 1;

                foreach (var kvp in weapons)
                {
                    InventoryItemUI itemUI;

                    if (!inventoryWeaponUIs.ContainsKey(kvp.Key))
                    {
                        itemUI = Instantiate(inventoryItemUIPrefab, container, false);

                        if (OnlinePlayer.Local.Inventory.ItemsDatabase.TryGetItem(kvp.Key, out InventoryItem item))
                            itemUI.SetIcon(item.ItemIcon);

                        itemUI.SetKeyCode(useKey.ToString());
                        inventoryWeaponUIs[kvp.Key] = itemUI;
                    }

                    if (inventoryWeaponUIs.TryGetValue(kvp.Key, out itemUI) && itemUI)
                    {
                        itemUI.SetQuantity(kvp.Value.TotalAmmo);
                        itemUI.gameObject.SetActive(kvp.Value.TotalAmmo > 0);
                    }

                    useKey++;
                }
            }
        }

        private void OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent()
        {
            container.RemoveChildren();
            inventoryWeaponUIs.Clear();
            OnlinePlayer.Local.Profile.Current.OnPropertyUpdatedEvent -= Profile_OnPropertyUpdatedEvent;
            OnlinePlayer.Local.Profile.OnProfileLoadedEvent.RemoveListener(OnProfileLoaded);
        }
    }
}
