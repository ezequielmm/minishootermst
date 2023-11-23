using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;

namespace MiniShooter
{
    public class NotificationRoomModule : BaseClientModule
    {
        private RoomServerManager roomServerManager;

        public override void OnInitialize(BaseClientBehaviour parentBehaviour)
        {
            roomServerManager = GetComponentInParent<RoomServerManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomConnectioId"></param>
        /// <param name="message"></param>
        public void NoticeToClient(int roomConnectioId, string message)
        {
            if (ParentBehaviour.Connection != null && ParentBehaviour.Connection.IsConnected)
            {
                if (roomServerManager.TryGetRoomPlayerByRoomPeer(roomConnectioId, out RoomPlayer roomPlayer))
                {
                    Mst.Server.Notifications.NotifyRecipient(roomPlayer.MasterPeerId, message, null);
                }
                else
                {
                    logger.Error($"Room player with id {roomConnectioId} is not found");
                }
            }
        }

        /// <summary>
        /// Notify master server that player died
        /// </summary>
        /// <param name="roomConnectioId"></param>
        public void NotifyPlayerDied(int roomConnectioId, IIdentifiable deathGiver)
        {
            if (ParentBehaviour.Connection != null && ParentBehaviour.Connection.IsConnected)
            {
                if (roomServerManager.TryGetRoomPlayerByRoomPeer(roomConnectioId, out RoomPlayer roomPlayer))
                {
                    ParentBehaviour.Connection.SendMessage(MstOpCodes.PlayerDied, roomPlayer.UserId, (status, message) =>
                    {
                        if (status != ResponseStatus.Success)
                        {
                            logger.Error(message.AsString());
                            return;
                        }

                        if (deathGiver != null)
                        {
                            Mst.Server.Notifications.NotifyRoom(roomServerManager.RoomController.RoomId,
                            new int[] { roomPlayer.MasterPeerId },
                            $"\"{roomPlayer.Username}\" was killed with \"{deathGiver.Title}\"", null);

                            Mst.Server.Notifications.NotifyRecipient(roomPlayer.MasterPeerId,
                                $"You are killed with \"{deathGiver.Title}\"!", null);
                        }
                        else
                        {
                            Mst.Server.Notifications.NotifyRoom(roomServerManager.RoomController.RoomId,
                            new int[] { roomPlayer.MasterPeerId },
                            $"Player {roomPlayer.Username} is killed", null);

                            Mst.Server.Notifications.NotifyRecipient(roomPlayer.MasterPeerId,
                                "You are killed!", null);
                        }
                    });
                }
                else
                {
                    logger.Error($"Room player with id {roomConnectioId} is not found");
                }
            }
        }
    }
}