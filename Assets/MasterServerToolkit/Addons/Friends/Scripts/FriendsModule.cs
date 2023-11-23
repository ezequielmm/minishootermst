using MasterServerToolkit.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MasterServerToolkit.MasterServer
{
    public class FriendsModule : BaseServerModule
    {
        #region INSPECTOR

        [Header("General Settings")]
        [SerializeField, Tooltip("If true, this module will subscribe to auth module, and automatically setup users when they log in")]
        protected bool useAuthModule = true;

        /// <summary>
        /// Database accessor factory that helps to create integration with friends db
        /// </summary>
        [Header("Components"), Tooltip("Database accessor factory that helps to create integration with friends db"), SerializeField]
        protected DatabaseAccessorFactory databaseAccessorFactory;

        #endregion

        /// <summary>
        /// Auth module for listening to auth events
        /// </summary>
        protected AuthModule authModule;

        /// <summary>
        /// Chat module for listening to chat events
        /// </summary>
        protected ChatModule chatModule;

        /// <summary>
        /// List of friends
        /// </summary>
        private readonly ConcurrentDictionary<string, FriendsListPacket> friends = new ConcurrentDictionary<string, FriendsListPacket>();

        /// <summary>
        /// 
        /// </summary>
        private readonly HashSet<string> friendsToRemove = new HashSet<string>();

        /// <summary>
        /// 
        /// </summary>
        private IFriendsDatabaseAccessor friendsDatabaseAccessor;

        public override void Initialize(IServer server)
        {
            if (databaseAccessorFactory != null)
                databaseAccessorFactory.CreateAccessors();

            // Get auth module dependency
            authModule = server.GetModule<AuthModule>();

            if (useAuthModule)
            {
                if (authModule)
                {
                    authModule.OnUserLoggedInEvent += OnUserLoggedInEventHandler;
                    authModule.OnUserLoggedOutEvent += OnUserLoggedOutEvent;
                }
                else
                {
                    logger.Error($"{GetType().Name} was set to use {nameof(AuthModule)}, but {nameof(AuthModule)} was not found");
                }
            }

            friendsDatabaseAccessor = Mst.Server.DbAccessors.GetAccessor<IFriendsDatabaseAccessor>();

            server.RegisterMessageHandler(MstOpCodes.GetFriends, GetFriendsMessageHandler);

            //server.RegisterMessageHandler(MstOpCodes.RequestFriendship, RequestFriendshipHandler);
            //server.RegisterMessageHandler((ushort)MstOpCodes.AcceptFriendship, AcceptFriendshipHandler);
            //server.RegisterMessageHandler((ushort)MstOpCodes.DeclineFriendship, DeclineFriendshipHandler);
            //server.RegisterMessageHandler((ushort)MstOpCodes.GetDeclinedFriendships, GetDeclinedFriendshipsHandler);


            //server.RegisterMessageHandler((ushort)MstOpCodes.InspectFriend, InspectFriendHandler);
            //server.RegisterMessageHandler((ushort)MstOpCodes.BlockFriends, BlockFriendsHandler);
            //server.RegisterMessageHandler((ushort)MstOpCodes.RemoveFriends, RemoveFriendsHandler);
        }

        public override MstProperties Info()
        {
            MstProperties info = base.Info();
            info.Set("Description", "This is a friends module that helps users to make friendship requests, accept friendships, and receive a list of friends.");
            info.Add("Database Accessor", databaseAccessorFactory != null ? "Connected" : "Not Connected");
            info.Add("Users with friends", friends.Count);
            return info;
        }

        protected virtual async void OnUserLoggedInEventHandler(IUserPeerExtension user)
        {
            // If friends list of this user is in the list of friends that should be deleted from use
            lock (friendsToRemove)
                friendsToRemove.Remove(user.UserId);

            // If friends are loaded
            if (friends.ContainsKey(user.UserId)) return;

            // Load all friends
            friends[user.UserId] = await friendsDatabaseAccessor.RestoreFriends(user.UserId);
        }

        protected virtual async void OnUserLoggedOutEvent(IUserPeerExtension user)
        {
            // Add this user to the list of friends to be deleted
            lock (friendsToRemove)
                friendsToRemove.Add(user.UserId);

            // Wait for a while
            await Task.Delay(1000 * 60);

            // Remove friends list
            lock (friendsToRemove)
            {
                if (friendsToRemove.Remove(user.UserId))
                    friends.TryRemove(user.UserId, out _);
            }
        }

        #region MESSAGE HANDLERS

        private void GetFriendsMessageHandler(IIncomingMessage message)
        {
            try
            {
                var userExtension = message.Peer.GetExtension<IUserPeerExtension>();

                if (userExtension == null)
                {
                    message.Respond(ResponseStatus.Unauthorized);
                    return;
                }


            }
            catch (Exception e)
            {
                logger.Error(e);
                message.Respond(ResponseStatus.Error);
            }
        }

        #endregion
    }
}