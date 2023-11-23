#if (!UNITY_WEBGL && !UNITY_IOS) || UNITY_EDITOR

using LiteDB;

namespace MasterServerToolkit.Bridges.LiteDB
{
    public class FriendsInfoData
    {
        [BsonId]
        public string UserId { get; set; }
        public byte[] Data { get; set; }
    }
}

#endif