using Mirror;
using System;
using UnityEngine;

namespace MiniShooter
{
    public delegate void PlayerCharacterDelegate(PlayerCharacter playerCharacter);

    [RequireComponent(typeof(OnlinePlayer))]
    public class OnlinePlayerCharacter : OnlinePlayerBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        protected PlayerCharacter playerCharacterPrefab;

        #endregion

        public PlayerCharacter Current { get; protected set; }

        public static event PlayerCharacterDelegate OnLocalCharacterCreatedEvent;
        public static event Action OnLocalCharacterDestroyedEvent;
        public static event Action OnLocalCharacterDiedEvent;
        public static event Action OnLocalCharacterDamageEvent;

        protected override void Awake()
        {
            base.Awake();
        }

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        [Command]
        private void Cmd_SpawnCharacter()
        {
            logger.Debug("Spawn character");

            Current = Instantiate(playerCharacterPrefab, transform.position, Quaternion.identity);
            Current.ServerPlayerCharacter = this;
            NetworkServer.Spawn(Current.gameObject, connectionToClient);
        }

        [Server]
        public void NotifyCharacterDied()
        {
            Target_NotifyCharacterDied(connectionToClient);
        }

        [Server]
        public void NotifyCharacterTakeDamage()
        {
            Target_NotifyCharacterTakeDamage(connectionToClient);
        }

        #endregion

        #region CLIENT

        public override void OnLocalPlayerReady()
        {
            base.OnLocalPlayerReady();
        }

        [TargetRpc]
        private void Target_NotifyCharacterDied(NetworkConnection conn)
        {
            OnLocalCharacterDiedEvent?.Invoke();
        }

        [TargetRpc]
        private void Target_NotifyCharacterTakeDamage(NetworkConnection conn)
        {
            OnLocalCharacterDamageEvent?.Invoke();
        }

        [Client]
        public void NotifiLocalCharacterCreated(PlayerCharacter playerCharacter)
        {
            Current = playerCharacter;
            OnLocalCharacterCreatedEvent.Invoke(playerCharacter);
        }

        [Client]
        public void NotifiLocalCharacterDestroyed()
        {
            OnLocalCharacterDestroyedEvent?.Invoke();
        }

        /// <summary>
        /// Creates player character on server
        /// </summary>
        [Client]
        public void Spawn()
        {
            if (isLocalPlayer)
                Cmd_SpawnCharacter();
        }

        #endregion
    }
}