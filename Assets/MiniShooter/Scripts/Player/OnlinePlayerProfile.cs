using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using UnityEngine.Events;

namespace MiniShooter
{
    public class OnlinePlayerProfile : OnlinePlayerBehaviour
    {
        #region INSPECTOR

        public UnityEvent OnProfileLoadedEvent;

        #endregion

        /// <summary>
        /// Client profile
        /// </summary>
        public ObservableProfile Current { get; protected set; }

        #region CLIENT

        public override void OnLocalPlayerReady()
        {
            base.OnLocalPlayerReady();

            Current = new ObservableProfile();
            ProfileProperties.Fill(Current);

            LoadProfile();
        }

        private void LoadProfile()
        {
            Mst.Client.Profiles.FillInProfileValues(Current, (isSuccessful, error) =>
            {
                if (isSuccessful)
                {
                    OnProfileLoadedEvent?.Invoke();
                }
                else
                {
                    logger.Error($"Could not load user profile. Error: {error}");
                }
            });
        }

        #endregion
    }
}