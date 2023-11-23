using MasterServerToolkit.Extensions;

namespace MasterServerToolkit.MasterServer
{
    public partial struct MstOpCodes
    {
        public static ushort GetFriends = nameof(GetFriends).ToUint16Hash();

        public static ushort FriendAdded = nameof(FriendAdded).ToUint16Hash();
        public static ushort RemoveFriends = nameof(RemoveFriends).ToUint16Hash();
        public static ushort InspectFriend = nameof(InspectFriend).ToUint16Hash();
        public static ushort BlockFriends = nameof(BlockFriends).ToUint16Hash();
        public static ushort RequestFriendship = nameof(RequestFriendship).ToUint16Hash();
        public static ushort AcceptFriendship = nameof(AcceptFriendship).ToUint16Hash();
        public static ushort IgnoreFriendship = nameof(IgnoreFriendship).ToUint16Hash();
        public static ushort GetDeclinedFriendships = nameof(GetDeclinedFriendships).ToUint16Hash();
        public static ushort DeclineFriendship = nameof(DeclineFriendship).ToUint16Hash();
    }
}
