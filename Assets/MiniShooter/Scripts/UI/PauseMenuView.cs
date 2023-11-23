using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;

namespace MiniShooter
{
    public class PauseMenuView : UIView
    {
        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            Mst.Events.Invoke(MstEventKeys.leaveRoom);
        }
    }
}