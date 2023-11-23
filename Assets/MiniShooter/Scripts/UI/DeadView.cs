using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using MasterServerToolkit.Utils;
using UnityEngine;

namespace MiniShooter
{
    public class DeadView : UIView
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private UIButton spawnButton;

        #endregion

        private TweenerActionInfo spawnTimerAction;

        protected override void Awake()
        {
            base.Awake();
            OnlinePlayerCharacter.OnLocalCharacterDiedEvent += OnlinePlayerCharacter_OnLocalCharacterDiedEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Tweener.Cancel(spawnTimerAction);
            OnlinePlayerCharacter.OnLocalCharacterDiedEvent -= OnlinePlayerCharacter_OnLocalCharacterDiedEvent;
        }

        private void OnlinePlayerCharacter_OnLocalCharacterDiedEvent()
        {
            ViewsManager.HideAllViews();
            Show();

            spawnButton.SetInteractable(false);

            spawnTimerAction = Tweener.Tween(5, 0, 5, (value) =>
            {
                spawnButton.SetLable($"Respawn in {value}");
            }).OnComplete((id) =>
            {
                spawnButton.SetLable("Respawn now");
                spawnButton.SetInteractable(true);
            });
        }

        public void Respawn()
        {
            Hide();
            OnlinePlayer.Local.Character.Spawn();
        }

        public void Quit()
        {
            Mst.Events.Invoke(MstEventKeys.leaveRoom);
        }
    }
}