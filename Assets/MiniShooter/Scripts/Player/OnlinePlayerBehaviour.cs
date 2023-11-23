using UnityEngine;

namespace MiniShooter
{
    [RequireComponent(typeof(OnlinePlayer))]
    public class OnlinePlayerBehaviour : NetworkEntityBehaviour
    {
        public OnlinePlayer Player { get; set; }

        public virtual void OnServerPlayerReady() { }

        public virtual void OnLocalPlayerReady() { }
    }
}