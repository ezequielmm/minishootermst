#if (!UNITY_WEBGL && !UNITY_IOS) || UNITY_EDITOR

using LiteDB;
using MasterServerToolkit.Logging;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using System;
using System.Threading.Tasks;

namespace MasterServerToolkit.Bridges.LiteDB
{
    public class FriendsDatabaseAccessor : IFriendsDatabaseAccessor
    {
        private ILiteCollection<FriendsInfoData> friends;
        private readonly ILiteDatabase database;

        public MstProperties CustomProperties { get; private set; } = new MstProperties();

        public FriendsDatabaseAccessor(string databaseName)
        {
            database = new LiteDatabase($"{databaseName}.db");
        }

        public void InitCollections()
        {
            friends = database.GetCollection<FriendsInfoData>("friends");
            friends.EnsureIndex(a => a.UserId, true);
        }

        public async Task<FriendsListPacket> RestoreFriends(string userId)
        {
            try
            {
                var data = await FindOrCreateData(userId);
                return SerializablePacket.FromBytes(data.Data, new FriendsListPacket());
            }
            catch (Exception e)
            {
                Logs.Error(e);
                return new FriendsListPacket();
            }
        }

        public async Task UpdateFriends(string userId, FriendsListPacket friendsList)
        {
            var data = await FindOrCreateData(userId);
            data.Data = friendsList.ToBytes();

            await Task.Run(() =>
            {
                friends?.Update(data);
            });
        }

        private async Task<FriendsInfoData> FindOrCreateData(string userId)
        {
            var data = await Task.Run(() =>
            {
                return friends.FindOne(a => a.UserId == userId);
            });

            if (data == null)
            {
                data = new FriendsInfoData()
                {
                    UserId = userId,
                    Data = new byte[0]
                };

                await Task.Run(() =>
                {
                    friends?.Insert(data);
                });
            }

            return data;
        }

        public void Dispose()
        {
            CustomProperties?.Clear();
            database?.Dispose();
        }
    }
}

#endif