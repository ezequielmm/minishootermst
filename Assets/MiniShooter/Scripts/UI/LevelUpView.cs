using MasterServerToolkit.Bridges;
using MasterServerToolkit.Extensions;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using MasterServerToolkit.UI;
using System.Collections.Generic;
using UnityEngine;

namespace MiniShooter
{
    public class LevelUpView : UIView
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private LevelUpCatalog levelUpCatalog;
        [SerializeField]
        private RectTransform levelUpsContainer;

        [Header("Prefabs"), SerializeField]
        private LevelUpInfoUI levelUpInfoUIPrefab;

        #endregion

        private ProfileLoaderBehaviour profileLoader;
        private readonly Dictionary<string, LevelUpInfoUI> levelUpButtons = new Dictionary<string, LevelUpInfoUI>();

        protected override void Start()
        {
            base.Start();

            profileLoader = FindObjectOfType<ProfileLoaderBehaviour>();
            profileLoader.OnProfileLoadedEvent.AddListener(OnProfileLoadedEventHandler);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnProfileLoadedEventHandler()
        {
            levelUpButtons.Clear();
            levelUpsContainer.RemoveChildren();

            foreach (LevelUpItemInfo info in levelUpCatalog.Items)
            {
                if (profileLoader.Profile.TryGet(info.key.ToUint16Hash(), out IObservableProperty property))
                    DrawButton(info, property);
            }
        }

        private void DrawButton(LevelUpItemInfo info, IObservableProperty property)
        {
            if (!levelUpButtons.ContainsKey(info.key))
                levelUpButtons[info.key] = Instantiate(levelUpInfoUIPrefab, levelUpsContainer, false);

            levelUpButtons[info.key].Set(info, property);
            levelUpButtons[info.key].OnClick(() =>
            {
                Mst.Client.Connection.SendMessage(MiniShooterOpCodes.LevelUp, info.key, (status, response) =>
                {
                    if (status != ResponseStatus.Success)
                    {
                        Mst.Events.Invoke(MstEventKeys.showOkDialogBox, new OkDialogBoxEventMessage()
                        {
                            Message = response.AsString()
                        });
                    }
                });
            });
        }
    }
}