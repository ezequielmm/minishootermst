using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using Mirror;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerCharacterAvatar : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Body"), SerializeField]
        private PlayerCharacterParts playerCharacterParts;

        #endregion

        protected RoomServerManager roomServerManager;
        protected RoomPlayer roomPlayer;

        protected readonly SyncDictionary<string, string> characterEditors = new SyncDictionary<string, string>();

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();

            roomServerManager = FindObjectOfType<RoomServerManager>();
            roomPlayer = roomServerManager.GetRoomPlayerByRoomPeer(connectionToClient.connectionId);

            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.characterEditor, out ObservableCharacterEditor property))
            {
                foreach (var kvp in property)
                    characterEditors[kvp.Key] = kvp.Value.ToBase64String();
            }
        }

        #endregion

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();

            characterEditors.Callback += CharacterEditors_OnChange;

            MstTimer.WaitForEndOfFrame(() =>
            {
                foreach (var kvp in characterEditors)
                {
                    CharacterEditors_OnChange(SyncIDictionary<string, string>.Operation.OP_ADD, kvp.Key, kvp.Value);
                }
            });
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            characterEditors.Callback -= CharacterEditors_OnChange;
        }

        private void CharacterEditors_OnChange(SyncIDictionary<string, string>.Operation op, string key, string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                var editor = SerializablePacket.FromBase64String(item, new CharacterEditorPacket());

                playerCharacterParts.SetPartActive(editor.Category, editor.PartId);

                for (int i = 0; i < editor.Colors.Count; i++)
                {
                    playerCharacterParts.SetPartColor(editor.Category, editor.PartId, i, editor.Colors[i].Color);
                }
            }
        }

        #endregion
    }
}