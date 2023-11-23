using MasterServerToolkit.MasterServer;

namespace MiniShooter
{
    public class ProfileRoomModule : BaseClientModule
    {
        private RoomServerManager roomServerManager;

        public override void OnInitialize(BaseClientBehaviour parentBehaviour)
        {
            if (!Mst.Client.Rooms.IsClientMode && parentBehaviour.TryGetComponent(out roomServerManager))
                roomServerManager.ProfileFactory = CreateRoomProfileFactory;
        }

        private ObservableServerProfile CreateRoomProfileFactory(string userId)
        {
            var profile = new ObservableServerProfile(userId);
            ProfileProperties.Fill(profile);
            return profile;
        }
    }
}