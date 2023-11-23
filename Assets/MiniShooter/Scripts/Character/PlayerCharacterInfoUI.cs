using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using Mirror;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerCharacterInfoUI : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private Canvas canvas;
        [SerializeField]
        private UIProperty healthProperty;
        [SerializeField]
        private UILable displayNameLable;
        [SerializeField]
        private PlayerCharacterVitals playerCharacterVitals;

        [Header("3D chat"), SerializeField]
        private ChatBubble3DUI chatBubble;
        [SerializeField]
        private float chatBubbleShowTime = 10f;

        #endregion

        protected RoomServerManager roomServerManager;
        protected RoomPlayer roomPlayer;
        protected Vector3 defaultScale = Vector3.one;
        protected float currentDistancePercentage = 0f;

        [SyncVar]
        protected string displayName = string.Empty;

        [SyncVar]
        protected string username = string.Empty;

        protected override void Awake()
        {
            base.Awake();

            defaultScale = canvas.transform.localScale;
        }

        private void Update()
        {
            if (isClient && !isOwned && Camera.main != null)
            {
                canvas.transform.rotation = Camera.main.transform.rotation;
            }
        }

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();

            roomServerManager = FindObjectOfType<RoomServerManager>();
            roomPlayer = roomServerManager.GetRoomPlayerByRoomPeer(connectionToClient.connectionId);

            username = roomPlayer.Username;

            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.displayName, out ObservableString displayNameProperty))
                displayName = displayNameProperty.Value;
        }

        #endregion

        #region CLIEN

        public override void OnStartClient()
        {
            base.OnStartClient();

            canvas.gameObject.SetActive(false);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            canvas.gameObject.SetActive(true);
            canvas.transform.rotation = Camera.main.transform.rotation;

            Mst.Client.Chat.OnMessageReceivedEvent += Chat_OnMessageReceivedEvent;

            InvokeRepeating(nameof(UpdateDisplayInfo), 0.2f, 0.2f);
        }

        private void Chat_OnMessageReceivedEvent(ChatMessagePacket message)
        {
            if (message.Sender == username)
            {
                chatBubble.Show(message.Message, chatBubbleShowTime);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            CancelInvoke();
            Mst.Client.Chat.OnMessageReceivedEvent -= Chat_OnMessageReceivedEvent;
        }

        private void UpdateDisplayInfo()
        {
            displayNameLable.Text = displayName;
            healthProperty.SetMax(playerCharacterVitals.MaxHealth);
            healthProperty.SetValue(playerCharacterVitals.Health);
        }

        #endregion
    }
}