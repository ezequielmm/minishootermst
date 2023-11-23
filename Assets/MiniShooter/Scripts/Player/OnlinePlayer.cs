using LiteDB;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using System;
using UnityEngine;

namespace MiniShooter
{
    public delegate void OnlinePlayerDelegate(OnlinePlayer player);

    public class OnlinePlayer : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private OnlinePlayerCharacter onlinePlayerCharacter;
        [SerializeField]
        private OnlinePlayerInput onlinePlayerInput;
        [SerializeField]
        private OnlinePlayerInventory onlinePlayerInventory;
        [SerializeField]
        private OnlinePlayerProfile onlinePlayerProfile;

        #endregion

        private OnlinePlayerBehaviour[] onlinePlayerBehaviours;
        private RoomPlayer roomPlayer;

        public OnlinePlayerCharacter Character => onlinePlayerCharacter;
        public OnlinePlayerInput Input => onlinePlayerInput;
        public OnlinePlayerInventory Inventory => onlinePlayerInventory;
        public OnlinePlayerProfile Profile => onlinePlayerProfile;

        /// <summary>
        /// 
        /// </summary>
        public RoomServerManager RoomServerManager { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public RoomPlayer RoomPlayer => roomPlayer;
        /// <summary>
        /// 
        /// </summary>
        public NotificationRoomModule NotificationRoomModule { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public static OnlinePlayer Local { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsServerReady => RoomServerManager && NotificationRoomModule && roomPlayer != null;

        /// <summary>
        /// Invokes when local player created
        /// </summary>
        public static event OnlinePlayerDelegate OnLocalPlayerCreatedEvent;
        /// <summary>
        /// Invokes when local player destroyed
        /// </summary>
        public static event Action OnLocalPlayerDestroyedEvent;

        protected override void Awake()
        {
            base.Awake();

            onlinePlayerBehaviours = GetComponents<OnlinePlayerBehaviour>();
        }

        private void OnDestroy()
        {
            if (isOwned)
                OnLocalPlayerDestroyedEvent?.Invoke();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            name = $"SERVER_PLAYER_{netId}";

            RoomServerManager = FindObjectOfType<RoomServerManager>();
            NotificationRoomModule = RoomServerManager.GetComponentInChildren<NotificationRoomModule>();

            MstTimer.WaitWhile(() => !RoomServerManager.TryGetRoomPlayerByRoomPeer(connectionToClient.connectionId, out roomPlayer), (isSuccess) =>
            {
                if (!isSuccess)
                {
                    logger.Error($"No room player found for client {connectionToClient.connectionId}");
                    connectionToClient.Disconnect();
                    return;
                }

                foreach (var playerBehaviour in onlinePlayerBehaviours)
                {
                    playerBehaviour.Player = this;
                    playerBehaviour.OnServerPlayerReady();
                }
            }, 5f);
        }

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();

            name = $"REMOTE_PLAYER_{netId}";
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            name = "LOCAL_PLAYER";

            Local = this;

            foreach (var playerBehaviour in onlinePlayerBehaviours)
            {
                playerBehaviour.Player = this;
                playerBehaviour.OnLocalPlayerReady();
            }

            OnLocalPlayerCreatedEvent?.Invoke(this);
        }

        #endregion
    }
}