using MasterServerToolkit.Networking;
using UnityEngine;

namespace MasterServerToolkit.MasterServer
{
    public class MstFriendsClient : MstBaseClient
    {
        public MstFriendsClient(IClientSocket connection) : base(connection) { }

        public void Test()
        {
            Connection.SendMessage(MstOpCodes.Ping, (status, response) =>
            {
                Debug.Log(response.AsString());
            });
        }
    }
}