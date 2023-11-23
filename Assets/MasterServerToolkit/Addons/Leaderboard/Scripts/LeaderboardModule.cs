using MasterServerToolkit.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MasterServerToolkit.MasterServer
{
    public class LeaderboardModule : BaseServerModule
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField, Min(5)]
        private int topLeaders = 10;

        /// <summary>
        /// Database accessor factory that helps to create integration with leaders db
        /// </summary>
        [Tooltip("Database accessor factory that helps to create integration with leaders db"), SerializeField]
        protected DatabaseAccessorFactory databaseAccessorFactory;

        #endregion

        protected readonly ConcurrentDictionary<string, List<ILeaderInfo>> leaderboards = new ConcurrentDictionary<string, List<ILeaderInfo>>();

        protected ILeaderboardDatabaseAccessor leaderboardDatabaseAccessor;

        public override void Initialize(IServer server)
        {
            if (databaseAccessorFactory != null)
                databaseAccessorFactory.CreateAccessors();

            leaderboardDatabaseAccessor = Mst.Server.DbAccessors.GetAccessor<ILeaderboardDatabaseAccessor>();

            if (leaderboardDatabaseAccessor == null)
            {
                logger.Fatal($"Leaderboard database implementation was not found in {GetType().Name}");
                return;
            }

            server.RegisterMessageHandler(MstOpCodes.GetLeaders, ClientGetLeadersMessageHandler);
        }

        #region MESSAGE HANDLERS

        private void ClientGetLeadersMessageHandler(IIncomingMessage message)
        {
            try
            {
                string leaderboard = message.AsString();

                if (!leaderboards.TryGetValue(leaderboard, out List<ILeaderInfo> leaders) || leaders == null)
                {
                    message.Respond($"Leaderboard {leaderboard} does not exist", ResponseStatus.NotFound);
                    return;
                }


            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                message.Respond(e.Message, ResponseStatus.Error);
            }
        }

        #endregion
    }
}