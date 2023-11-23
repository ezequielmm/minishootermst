using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using Mirror.SimpleWeb;

namespace MiniShooter
{
    public class RoomServerManager : MasterServerToolkit.MasterServer.RoomServerManager
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();

            var wsTransport = GetComponent<SimpleWebTransport>();

            if (Mst.Client.Rooms.IsClientMode && Mst.Settings.UseSecure)
            {
                wsTransport.sslEnabled = false;
                wsTransport.clientUseWss = true;
            }
            else if (!Mst.Client.Rooms.IsClientMode && Mst.Settings.UseSecure)
            {
                wsTransport.sslEnabled = true;
                wsTransport.clientUseWss = false;
            }
            else
            {
                wsTransport.sslEnabled = false;
                wsTransport.clientUseWss = false;
            }
        }

        /// <summary>
        /// Invoked when player joins a room
        /// </summary>
        /// <param name="player"></param>
        protected override void OnPlayerJoinedRoom(RoomPlayer player)
        {
            if (player.Profile.TryGet(ProfilePropertyKeys.displayName, out ObservableString displayName))
            {
                MstTimer.WaitForSeconds(2f, () =>
                {
                    Mst.Server.Notifications.NotifyRecipient(player.MasterPeerId,
                                $"Hi, {displayName.Value}!\nWelcome to \"{RoomOptions.Name}\" server", null);
                });

                Mst.Server.Notifications.NotifyRoom(RoomController.RoomId,
                        new int[] { player.MasterPeerId },
                        $"Player {displayName.Value} has just joined the room",
                        null);
            }
            else
            {
                base.OnPlayerJoinedRoom(player);
            }
        }

        /// <summary>
        /// Invoked when player leaves a room
        /// </summary>
        /// <param name="player"></param>
        protected override void OnPlayerLeftRoom(RoomPlayer player)
        {
            if (player.Profile.TryGet(ProfilePropertyKeys.displayName, out ObservableString displayName))
            {
                Mst.Server.Notifications.NotifyRoom(RoomController.RoomId,
                        new int[] { player.MasterPeerId },
                        $"Player {displayName.Value} has just left the room",
                        null);
            }
            else
            {
                base.OnPlayerLeftRoom(player);
            }
        }

        protected override void BeforeRoomRegistering()
        {
            base.BeforeRoomRegistering();

            RoomOptions.CustomOptions.Set("rule1", 1);
            RoomOptions.CustomOptions.Set("rule2", 32);
            RoomOptions.CustomOptions.Set("rule3", 12);
        }
    }
}
