using MasterServerToolkit.Bridges;
using MasterServerToolkit.Extensions;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace MiniShooter
{
    public class ChatView : UIView
    {
        #region INSPECTOR

        [Header("Setting"), SerializeField, Range(5, 40)]
        private int maxMessagesHistory = 20;

        [Header("Prefabs"), SerializeField]
        private ChatMessageItemUI incomingMessageUIPrefab;
        [SerializeField]
        private ChatMessageItemUI outgoingMessageUIPrefab;

        [Header("Components"), SerializeField]
        private RectTransform messagesContainer;
        [SerializeField]
        private TMP_InputField messageInputField;

        #endregion

        private readonly Queue<ChatMessageItemUI> messageInstances = new Queue<ChatMessageItemUI>();

        private OnlinePlayerInput playerInput;
        private string roomChannel = string.Empty;

        protected override void Awake()
        {
            base.Awake();

            SetActiveInputField(false);
            messagesContainer.RemoveChildren();

            OnlinePlayer.OnLocalPlayerCreatedEvent += OnlinePlayer_OnLocalPlayerCreatedEvent;
            OnlinePlayer.OnLocalPlayerDestroyedEvent += OnlinePlayer_OnLocalPlayerDestroyedEvent;

            Mst.Client.Chat.OnMessageReceivedEvent += Chat_OnMessageReceivedEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnlinePlayer.OnLocalPlayerCreatedEvent -= OnlinePlayer_OnLocalPlayerCreatedEvent;
            OnlinePlayer.OnLocalPlayerDestroyedEvent -= OnlinePlayer_OnLocalPlayerDestroyedEvent;

            Mst.Client.Chat.OnMessageReceivedEvent -= Chat_OnMessageReceivedEvent;
            Mst.Client.Chat.LeaveChannel(roomChannel, null);
        }

        protected override void OnHide()
        {
            base.OnHide();

            SetActiveInputField(false);
        }

        private void OnlinePlayer_OnLocalPlayerCreatedEvent(OnlinePlayer player)
        {
            roomChannel = $"room_{Mst.Client.Rooms.ReceivedAccess.RoomId}";

            Mst.Client.Chat.JoinChannel(roomChannel, (isSuccess, error) =>
            {
                if (!isSuccess)
                {
                    logger.Error(error);
                }
                else
                {
                    logger.Info($"You have successfully joined the chat channel {roomChannel}");
                }
            });

            playerInput = player.GetComponent<OnlinePlayerInput>();
            playerInput.Actions.Player.ChatInputField.performed += ChatInputField_performed;
            playerInput.Actions.UI.ChatSubmit.performed += ChatSubmit_performed;
        }

        private void OnlinePlayer_OnLocalPlayerDestroyedEvent()
        {
            playerInput.Actions.Player.ChatInputField.performed -= ChatInputField_performed;
            playerInput.Actions.UI.ChatSubmit.performed -= ChatSubmit_performed;
        }

        private void ChatInputField_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!IsVisible)
                Show();

            if (!IsActiveInputField())
                SetActiveInputField(true);
        }

        private void ChatSubmit_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (IsActiveInputField())
            {
                string message = messageInputField.text;

                Regex re = new Regex(@"\s{2,}");
                message = re.Replace(message, " ");

                if (messageInputField.text.Trim().Length > 0)
                    SendChannelMessage(message);

                messageInputField.text = string.Empty;
                SetActiveInputField(false);
            }
        }

        private void Chat_OnMessageReceivedEvent(ChatMessagePacket message)
        {
            bool isLocalPlayer = Mst.Client.Auth.IsSignedIn && message.Sender == Mst.Client.Auth.AccountInfo.Username;

            if (isLocalPlayer)
            {
                CreateMessageInstance(outgoingMessageUIPrefab, message, isLocalPlayer);
            }
            else
            {
                CreateMessageInstance(incomingMessageUIPrefab, message, false);
            }
        }

        private bool IsActiveInputField()
        {
            return messageInputField != null && messageInputField.gameObject.activeSelf;
        }

        private void SetActiveInputField(bool value)
        {
            if (messageInputField == null) return;

            messageInputField.gameObject.SetActive(value);

            if (value)
            {
                messageInputField.Select();
                messageInputField.ActivateInputField();

                if (playerInput)
                    playerInput.EnableUiInput();
            }
            else
            {
                messageInputField.DeactivateInputField();

                if (playerInput)
                    playerInput.EnablePlayerInput();
            }
        }

        private void CreateMessageInstance(ChatMessageItemUI messageUIPrefab, ChatMessagePacket message, bool isLocalPlayer)
        {
            if (messageInstances.Count >= maxMessagesHistory)
                Destroy(messageInstances.Dequeue().gameObject);

            var messageInstance = Instantiate(messageUIPrefab, messagesContainer, false);

            if (isLocalPlayer)
            {
                messageInstance.Set("Me", message.Message);
            }
            else
            {
                messageInstance.Set(message.Sender, message.Message);
            }

            messageInstances.Enqueue(messageInstance);
        }

        public void SendChannelMessage(string messageText)
        {
            Mst.Client.Chat.SendChannelMessage(roomChannel, messageText, null);
        }
    }
}