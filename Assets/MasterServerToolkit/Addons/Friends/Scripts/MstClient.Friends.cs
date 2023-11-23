namespace MasterServerToolkit.MasterServer
{
    public partial class MstClient : MstBaseClient
    {
        private MstFriendsClient _friends;

        public MstFriendsClient Friends
        {
            get
            {
                if (_friends == null)
                    _friends = new MstFriendsClient(Connection);

                return _friends;
            }
        }
    }
}