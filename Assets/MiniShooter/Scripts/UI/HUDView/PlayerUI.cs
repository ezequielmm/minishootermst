using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private AvatarComponent avatarUI;
        [SerializeField]
        private UILable displayNameLable;
        [SerializeField]
        private UIProperty healthPropertyUI;
        [SerializeField]
        private UIProperty staminaPropertyUI;
        [SerializeField]
        private GameObject isStaminaFrozenUI;

        #endregion

        private void Awake()
        {
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
            if (playerCharacter.TryGetComponent(out PlayerCharacterVitals characterVitals))
            {
                UpdatePropertiesInfo(characterVitals);

                characterVitals.OnParamsChangeEvent.AddListener(() =>
                {
                    UpdatePropertiesInfo(characterVitals);
                });
            }

            if (OnlinePlayer.Local.Profile.Current != null)
                OnProfileLoaded();

            OnlinePlayer.Local.Profile.OnProfileLoadedEvent.AddListener(OnProfileLoaded);
        }

        private void UpdatePropertiesInfo(PlayerCharacterVitals characterVitals)
        {
            isStaminaFrozenUI.SetActive(characterVitals.IsStaminaFrozen);
            healthPropertyUI.SetValue(characterVitals.Health);
            staminaPropertyUI.SetValue(characterVitals.Stamina);
        }

        private void OnProfileLoaded()
        {
            // Draw all info in UI 
            foreach (var property in OnlinePlayer.Local.Profile.Current.Properties)
                Profile_OnPropertyUpdatedEvent(property.Key, property.Value);

            // Listen to profile events
            OnlinePlayer.Local.Profile.Current.OnPropertyUpdatedEvent += Profile_OnPropertyUpdatedEvent;
        }

        private void OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent()
        {
            OnlinePlayer.Local.Profile.OnProfileLoadedEvent.RemoveListener(OnProfileLoaded);

            if (OnlinePlayer.Local.Profile.Current != null)
                OnlinePlayer.Local.Profile.Current.OnPropertyUpdatedEvent -= Profile_OnPropertyUpdatedEvent;
        }

        private void Profile_OnPropertyUpdatedEvent(ushort propertyCode, IObservableProperty property)
        {
            if (propertyCode == ProfilePropertyKeys.avatarUrl)
            {
                avatarUI.SetAvatarUrl(property.As<ObservableString>().Value);
            }
            else if (propertyCode == ProfilePropertyKeys.displayName)
            {
                displayNameLable.Text = property.As<ObservableString>().Value;
            }
            else if (propertyCode == ProfilePropertyKeys.maxHealth)
            {
                healthPropertyUI.SetMin(0f);
                healthPropertyUI.SetMax(property.As<ObservableFloat>().Value);
            }
            else if (propertyCode == ProfilePropertyKeys.maxStamina)
            {
                staminaPropertyUI.SetMin(0f);
                staminaPropertyUI.SetMax(property.As<ObservableFloat>().Value);
            }
        }
    }
}
