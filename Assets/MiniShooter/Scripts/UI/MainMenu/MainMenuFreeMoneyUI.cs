using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using MasterServerToolkit.UI;
using System;
using UnityEngine;

namespace MiniShooter
{
    public class MainMenuFreeMoneyUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private UIButton getButton;
        [SerializeField]
        private ProfilesSettings profilesSettings;

        #endregion

        private ProfileLoaderBehaviour profileLoader;
        private DateTime nextFreeMoneyReceiveTime;

        private void Awake()
        {
            nextFreeMoneyReceiveTime = DateTime.MinValue;
        }

        protected void Start()
        {
            profileLoader = FindObjectOfType<ProfileLoaderBehaviour>();
            profileLoader.OnProfileLoadedEvent.AddListener(OnProfileLoadedEventHandler);
        }

        protected void Update()
        {
            if (getButton != null)
            {
                if (DateTime.UtcNow < nextFreeMoneyReceiveTime)
                {
                    var ts = (nextFreeMoneyReceiveTime - DateTime.UtcNow);
                    getButton.SetLable($"Get in {ts:mm\\:ss}");
                }
                else
                {
                    getButton.SetLable($"Get +{profilesSettings.freeMoney}");
                }
            }
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
            if (property.Key == ProfilePropertyKeys.nextFreeMoneyReceiveTime)
            {
                nextFreeMoneyReceiveTime = property.As<ObservableDateTime>().Value;
            }
        }

        public void GetFreeMoney()
        {
            Mst.Client.Connection.SendMessage(MiniShooterOpCodes.GetFreeMoney, (status, response) =>
            {
                if (status != ResponseStatus.Success)
                {
                    Mst.Events.Invoke(MstEventKeys.showOkDialogBox, new OkDialogBoxEventMessage()
                    {
                        Message = response.AsString()
                    });
                }
            });
        }
    }
}
