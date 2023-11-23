using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using TMPro;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerEditorView : UIView
    {
        #region INSPECTOR

        [Header("Editor"), SerializeField]
        private TMP_InputField displayNameInputField;
        [SerializeField]
        private TMP_InputField avatarUrlInputField;

        #endregion

        private ProfileLoaderBehaviour profileLoader;

        protected override void Start()
        {
            base.Start();

            profileLoader = FindObjectOfType<ProfileLoaderBehaviour>();
            profileLoader.OnProfileLoadedEvent.AddListener(OnProfileLoadedEventHandler);
        }

        private void OnProfileLoadedEventHandler()
        {
            foreach (var property in profileLoader.Profile.Properties)
                DrawProfileProperty(property.Key, property.Value);
        }

        private void DrawProfileProperty(ushort propertyCode, IObservableProperty property)
        {
            if (propertyCode == ProfilePropertyKeys.avatarUrl)
            {
                avatarUrlInputField.text = property.As<ObservableString>().Value;
            }
            else if (propertyCode == ProfilePropertyKeys.displayName)
            {
                displayNameInputField.text = property.As<ObservableString>().Value;
            }
        }

        public void Submit()
        {
            Mst.Client.Connection.SendMessage(MstOpCodes.UpdateDisplayNameRequest,
                    displayNameInputField.text, (status, response) => { });

            Mst.Client.Connection.SendMessage(MstOpCodes.UpdateAvatarRequest,
                    avatarUrlInputField.text, (status, response) => { });
        }
    }
}