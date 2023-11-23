using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;

namespace MiniShooter
{
    public class ProfilesBehaviour : ProfileLoaderBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            Profile = new ObservableProfile();
            ProfileProperties.Fill(Profile);
        }
    }
}