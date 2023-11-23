using LiteDB;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerCharacter : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private string characterTitle = "Character";

        #endregion

        public string Title => characterTitle;
        public OnlinePlayerCharacter ServerPlayerCharacter { get; set; }

        protected override void Awake()
        {
            base.Awake();
        }

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();
            name = $"SERVER_PLAYER_CHARACTER_{netId}";
        }

        #endregion

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();

            if(!isOwned)
            {
                name = $"REMOTE_PLAYER_CHARACTER_{netId}";
            }
            
            logger.Debug($"Character {name} created");
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            name = $"LOCAL_PLAYER_CHARACTER";
            OnlinePlayer.Local.Character.NotifiLocalCharacterCreated(this);
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            OnlinePlayer.Local.Character.NotifiLocalCharacterDestroyed();
        }

        #endregion
    }
}