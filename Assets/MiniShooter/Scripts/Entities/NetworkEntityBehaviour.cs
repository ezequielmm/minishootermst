using MasterServerToolkit.Logging;
using MasterServerToolkit.MasterServer;
using Mirror;
using System;
using UnityEngine;

namespace MiniShooter
{
    public class NetworkEntityBehaviour : NetworkBehaviour, IEquatable<NetworkEntityBehaviour>
    {
        #region INSPECTOR

        /// <summary>
        /// Log levelof this module
        /// </summary>
        [Header("Base Settings"), SerializeField]
        protected LogLevel logLevel = LogLevel.Info;

        #endregion

        /// <summary>
        /// Logger assigned to this module
        /// </summary>
        protected MasterServerToolkit.Logging.Logger logger;

        /// <summary>
        /// Check if this behaviour is ready
        /// </summary>
        public virtual bool IsReady { get; protected set; } = true;

        protected virtual void Awake()
        {
            logger = Mst.Create.Logger(GetType().Name);
            logger.LogLevel = logLevel;
        }

        public bool Equals(NetworkEntityBehaviour other)
        {
            if (other)
                return netId == other.netId;

            return false;
        }
    }
}