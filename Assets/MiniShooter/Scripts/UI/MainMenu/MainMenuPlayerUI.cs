using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using UnityEngine;

namespace MiniShooter
{
    public class MainMenuPlayerUI : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private AvatarComponent avatarUI;
        [SerializeField]
        private UILable usernameLable;
        [SerializeField]
        private UILable displayNameLable;

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
            usernameLable.Text = Mst.Client.Auth.AccountInfo.Username;

            profileLoader.Profile.OnPropertyUpdatedEvent += Profile_OnPropertyUpdatedEvent;

            foreach (var property in profileLoader.Profile.Properties)
                Profile_OnPropertyUpdatedEvent(property.Key, property.Value);
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
        }
    }
}