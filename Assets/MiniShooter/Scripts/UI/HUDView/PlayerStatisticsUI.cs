using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerStatisticsUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private UIProperty moneyPropertyUI;
        [SerializeField]
        private UIProperty totalKillsPropertyUI;
        [SerializeField]
        private UIProperty totalDeathsPropertyUI;

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
            if (OnlinePlayer.Local.Profile.Current != null)
                OnProfileLoaded();

            OnlinePlayer.Local.Profile.OnProfileLoadedEvent.AddListener(OnProfileLoaded);
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
            if (property.Key == ProfilePropertyKeys.money)
            {
                moneyPropertyUI.SetValue(property.As<ObservableInt>().Value);
            }
            else if (property.Key == ProfilePropertyKeys.totalKills)
            {
                totalKillsPropertyUI.SetValue(property.As<ObservableInt>().Value);
            }
            else if (property.Key == ProfilePropertyKeys.totalDeaths)
            {
                totalDeathsPropertyUI.SetValue(property.As<ObservableInt>().Value);
            }
        }
    }
}