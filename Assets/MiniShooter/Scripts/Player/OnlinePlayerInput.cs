using UnityEngine;

namespace MiniShooter
{
    [RequireComponent(typeof(OnlinePlayer))]
    public class OnlinePlayerInput : OnlinePlayerBehaviour
    {
        private PlayerControls inputActions;
        public PlayerControls Actions => inputActions;

        protected override void Awake()
        {
            base.Awake();

#if ENABLE_INPUT_SYSTEM
            inputActions = new PlayerControls();
            inputActions.Disable();
#endif
        }

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();

#if ENABLE_INPUT_SYSTEM
            inputActions.Player.Enable();
#else
			logger.Error("This behaviour is missing dependencies. Please install Input System");
#endif
        }

        public override void OnLocalPlayerReady()
        {
            base.OnLocalPlayerReady();
        }

        public void EnablePlayerInput()
        {
#if ENABLE_INPUT_SYSTEM
            inputActions.UI.Disable();
            inputActions.Player.Enable();
#else
			logger.Error("This behaviour is missing dependencies. Please install Input System");
#endif
        }

        public void EnableUiInput()
        {
#if ENABLE_INPUT_SYSTEM
            inputActions.Player.Disable();
            inputActions.UI.Enable();
#else
			logger.Error("This behaviour is missing dependencies. Please install Input System");
#endif
        }

        #endregion
    }
}