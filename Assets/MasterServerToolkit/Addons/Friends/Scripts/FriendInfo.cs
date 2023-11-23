using System.Collections.Generic;

namespace MasterServerToolkit.MasterServer
{
    public class FriendInfo
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public FriendStatus Status { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}