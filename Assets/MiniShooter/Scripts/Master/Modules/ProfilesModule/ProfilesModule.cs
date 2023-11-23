using MasterServerToolkit.Extensions;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using MasterServerToolkit.Utils;
using System;
using UnityEngine;

namespace MiniShooter
{
    public class ProfilesModule : MasterServerToolkit.MasterServer.ProfilesModule
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private ProfilesSettings profileSettings;
        [SerializeField]
        private LevelUpCatalog levelUpCatalog;

        #endregion

        public override void Initialize(IServer server)
        {
            base.Initialize(server);

            // Set the new factory in ProfilesModule
            ProfileFactory = CreateProfileInServer;

            server.RegisterMessageHandler(MstOpCodes.PlayerDied, OnRoomCharacterDiedMessageHandler);

            server.RegisterMessageHandler(MstOpCodes.UpdateDisplayNameRequest, UpdateDisplayNameRequestHandler);
            server.RegisterMessageHandler(MstOpCodes.UpdateAvatarRequest, UpdateAvatarRequestHandler);

            server.RegisterMessageHandler(MiniShooterOpCodes.GetFreeMoney, GetFreeMoneyMessageHandler);
            server.RegisterMessageHandler(MiniShooterOpCodes.LevelUp, LevelUpMessageHandler);

            server.RegisterMessageHandler(MiniShooterOpCodes.UpdateCharacterEditorInfo, UpdateCharacterPartMessageHandler);
        }

        /// <summary>
        /// This method just creates a new profile on server side as default for users that are logged in for the first time
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientPeer"></param>
        /// <returns></returns>
        private ObservableServerProfile CreateProfileInServer(string userId, IPeer clientPeer)
        {
            string displayName = SimpleNameGenerator.Generate(Gender.Male);
            string avatarUrl = $"https://i.pravatar.cc/150?img={Mst.Helper.Random.Next(1, 71)}";

            var profile = new ObservableServerProfile(userId, clientPeer);
            ProfileProperties.Fill(profile);

            profile.Get<ObservableString>(ProfilePropertyKeys.displayName).Value = displayName;
            profile.Get<ObservableString>(ProfilePropertyKeys.avatarUrl).Value = avatarUrl;

            if (profile.TryGet(ProfilePropertyKeys.weapons, out ObservableWeapons weapons))
            {
                foreach (var sturtupWeapon in profileSettings.weaponItems)
                {
                    weapons[sturtupWeapon.item.ItemId] = new WeaponPacket()
                    {
                        CurrentAmmo = sturtupWeapon.currentAmmo,
                        TotalAmmo = sturtupWeapon.totalAmmo
                    };
                }
            }

            if (profile.TryGet(ProfilePropertyKeys.items, out ObservableDictStringInt items))
            {
                foreach (var sturtupItem in profileSettings.genericItems)
                {
                    items[sturtupItem.item.ItemId] = sturtupItem.quantity;
                }
            }

            if (profile.TryGet(ProfilePropertyKeys.money, out ObservableInt money))
                money.Add(profileSettings.startMoney);

            if (profile.TryGet(ProfilePropertyKeys.maxHealth, out ObservableFloat maxHealth))
                maxHealth.Add(profileSettings.maxHealth);

            if (profile.TryGet(ProfilePropertyKeys.health, out ObservableFloat health))
                health.Add(profileSettings.health);

            if (profile.TryGet(ProfilePropertyKeys.restoreHealthMultiplier, out ObservableFloat restoreHealthMultiplier))
                restoreHealthMultiplier.Add(profileSettings.restoreHealthMultiplier);

            if (profile.TryGet(ProfilePropertyKeys.maxStamina, out ObservableFloat maxStamina))
                maxStamina.Add(profileSettings.maxStamina);

            if (profile.TryGet(ProfilePropertyKeys.stamina, out ObservableFloat stamina))
                stamina.Add(profileSettings.stamina);

            if (profile.TryGet(ProfilePropertyKeys.restoreStaminaMultiplier, out ObservableFloat restoreStaminaMultiplier))
                restoreStaminaMultiplier.Add(profileSettings.restoreStaminaMultiplier);

            return profile;
        }

        #region MESSAGE HANDLERS

        /// <summary>
        /// Invoked when player character dies on server room
        /// </summary>
        /// <param name="message"></param>
        private void OnRoomCharacterDiedMessageHandler(IIncomingMessage message)
        {
            message.Respond(ResponseStatus.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void UpdateDisplayNameRequestHandler(IIncomingMessage message)
        {
            var userExtension = message.Peer.GetExtension<IUserPeerExtension>();

            if (userExtension == null || userExtension.Account == null)
            {
                message.Respond("Invalid session", ResponseStatus.Unauthorized);
                return;
            }

            try
            {
                if (profilesList.TryGetValue(userExtension.UserId, out ObservableServerProfile profile))
                {
                    profile.Get<ObservableString>(ProfilePropertyKeys.displayName).Value = message.AsString();
                    message.Respond(ResponseStatus.Success);
                }
                else
                {
                    message.Respond("Invalid session", ResponseStatus.Unauthorized);
                }
            }
            catch (Exception e)
            {
                message.Respond($"Internal Server Error: {e}", ResponseStatus.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void UpdateAvatarRequestHandler(IIncomingMessage message)
        {
            var userExtension = message.Peer.GetExtension<IUserPeerExtension>();

            if (userExtension == null || userExtension.Account == null)
            {
                message.Respond("Invalid session", ResponseStatus.Unauthorized);
                return;
            }

            try
            {
                if (profilesList.TryGetValue(userExtension.UserId, out ObservableServerProfile profile))
                {
                    profile.Get<ObservableString>(ProfilePropertyKeys.avatarUrl).Value = message.AsString();
                    message.Respond(ResponseStatus.Success);
                }
                else
                {
                    message.Respond("Invalid session", ResponseStatus.Unauthorized);
                }
            }
            catch (Exception e)
            {
                message.Respond($"Internal Server Error: {e}", ResponseStatus.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateCharacterPartMessageHandler(IIncomingMessage message)
        {
            var userExtension = message.Peer.GetExtension<IUserPeerExtension>();

            if (userExtension == null || userExtension.Account == null)
            {
                message.Respond(ResponseStatus.Unauthorized);
                return;
            }

            try
            {
                if (profilesList.TryGetValue(userExtension.UserId, out ObservableServerProfile profile) && profile != null)
                {
                    var characterEditorInfo = message.AsPacket(new CharacterEditorPacket());

                    if (profile.TryGet(ProfilePropertyKeys.characterEditor, out ObservableCharacterEditor property))
                    {
                        property[characterEditorInfo.Category] = characterEditorInfo;
                    }

                    message.Respond(ResponseStatus.Success);
                }
                else
                {
                    message.Respond(ResponseStatus.Unauthorized);
                }
            }
            catch (Exception e)
            {
                message.Respond($"Internal Server Error: {e}", ResponseStatus.Error);
            }
        }

        private void GetFreeMoneyMessageHandler(IIncomingMessage message)
        {
            var userExtension = message.Peer.GetExtension<IUserPeerExtension>();

            if (userExtension == null || userExtension.Account == null)
            {
                message.Respond(ResponseStatus.Unauthorized);
                return;
            }

            try
            {
                if (profilesList.TryGetValue(userExtension.UserId, out ObservableServerProfile profile) && profile != null)
                {
                    if (profile.TryGet(ProfilePropertyKeys.nextFreeMoneyReceiveTime, out ObservableDateTime property))
                    {
                        if (property.Value <= DateTime.UtcNow)
                        {
                            if (profile.TryGet(ProfilePropertyKeys.money, out ObservableInt money))
                                money.Add(profileSettings.freeMoney);

                            property.Value = DateTime.UtcNow.AddSeconds(profileSettings.freeMoneyRate);
                            message.Respond(ResponseStatus.Success);
                        }
                        else
                        {
                            var ts = (property.Value - DateTime.UtcNow);
                            message.Respond($"It's not time to get free money. Try later in {ts:mm\\:ss}", ResponseStatus.Failed);
                        }
                    }
                    else
                    {
                        message.Respond("Property not found", ResponseStatus.NotFound);
                    }
                }
                else
                {
                    message.Respond(ResponseStatus.Unauthorized);
                }
            }
            catch (Exception e)
            {
                message.Respond($"Internal Server Error: {e}", ResponseStatus.Error);
            }
        }

        private void LevelUpMessageHandler(IIncomingMessage message)
        {
            var userExtension = message.Peer.GetExtension<IUserPeerExtension>();

            if (userExtension == null || userExtension.Account == null)
            {
                message.Respond(ResponseStatus.Unauthorized);
                return;
            }

            try
            {
                if (profilesList.TryGetValue(userExtension.UserId, out ObservableServerProfile profile) && profile != null)
                {
                    string propertyKey = message.AsString();

                    if (levelUpCatalog.TryGetLevelUpInfo(propertyKey, out LevelUpItemInfo lvlUpInfo)
                        && profile.TryGet(propertyKey.ToUint16Hash(), out ObservableFloat lvlUpProperty))
                    {
                        if (lvlUpProperty.Value >= lvlUpInfo.max)
                        {
                            message.Respond("You have reached max value", ResponseStatus.Failed);
                            return;
                        }

                        // Try get money
                        if (profile.TryGet(ProfilePropertyKeys.money, out ObservableInt moneyProperty))
                        {
                            // Try take some money
                            if (moneyProperty.Subtract(lvlUpInfo.price, 0))
                            {
                                lvlUpProperty.Value += lvlUpInfo.value;
                            }
                            else
                            {
                                message.Respond("Not enough money", ResponseStatus.Failed);
                            }
                        }
                        else
                        {
                            logger.Error("Money property not found");
                            message.Respond(ResponseStatus.Failed);
                        }
                    }
                    else
                    {
                        logger.Error($"Property or info for key {propertyKey} not found");
                        message.Respond(ResponseStatus.Failed);
                    }
                }
                else
                {
                    message.Respond(ResponseStatus.Unauthorized);
                }
            }
            catch (Exception e)
            {
                message.Respond($"Internal Server Error: {e}", ResponseStatus.Error);
            }
        }

        #endregion
    }
}