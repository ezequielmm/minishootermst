using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using UnityEngine;

namespace MiniShooter
{
    public class MainMenuPlayerStatisticsUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private UIProperty moneyPropertyUI;
        [SerializeField]
        private UIProperty totalKillsPropertyUI;
        [SerializeField]
        private UIProperty totalDeathsPropertyUI;

        #endregion

        private ProfileLoaderBehaviour profileLoader;

        protected void Start()
        {
            profileLoader = FindObjectOfType<ProfileLoaderBehaviour>();
            profileLoader.OnProfileLoadedEvent.AddListener(OnProfileLoadedEventHandler);
        }

        protected void OnDestroy()
        {
            if (profileLoader && profileLoader.Profile != null)
                profileLoader.Profile.OnPropertyUpdatedEvent -= Profile_OnPropertyUpdatedEvent;
        }

        private void OnProfileLoadedEventHandler()
        {
            profileLoader.Profile.OnPropertyUpdatedEvent += Profile_OnPropertyUpdatedEvent;

            foreach (var property in profileLoader.Profile.Properties)
                Profile_OnPropertyUpdatedEvent(property.Key, property.Value);
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