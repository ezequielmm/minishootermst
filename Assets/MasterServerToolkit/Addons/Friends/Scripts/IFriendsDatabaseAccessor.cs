using System.Threading.Tasks;

namespace MasterServerToolkit.MasterServer
{
    public interface IFriendsDatabaseAccessor : IDatabaseAccessor
    {
        Task<FriendsListPacket> RestoreFriends(string userId);
        Task UpdateFriends(string userId, FriendsListPacket friends);
    }
}