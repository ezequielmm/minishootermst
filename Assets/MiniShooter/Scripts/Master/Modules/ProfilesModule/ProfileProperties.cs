using MasterServerToolkit.MasterServer;
using System;

namespace MiniShooter
{
    public static class ProfileProperties
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        public static void Fill(ObservableProfile profile)
        {
            profile.Add(new ObservableString(ProfilePropertyKeys.displayName));
            profile.Add(new ObservableString(ProfilePropertyKeys.avatarUrl));
            profile.Add(new ObservableInt(ProfilePropertyKeys.totalKills));
            profile.Add(new ObservableInt(ProfilePropertyKeys.totalDeaths));
            profile.Add(new ObservableInt(ProfilePropertyKeys.money));

            profile.Add(new ObservableFloat(ProfilePropertyKeys.health));
            profile.Add(new ObservableFloat(ProfilePropertyKeys.maxHealth));

            profile.Add(new ObservableFloat(ProfilePropertyKeys.stamina));
            profile.Add(new ObservableFloat(ProfilePropertyKeys.maxStamina));

            profile.Add(new ObservableCharacterEditor(ProfilePropertyKeys.characterEditor));
            profile.Add(new ObservableWeapons(ProfilePropertyKeys.weapons));
            profile.Add(new ObservableDictStringInt(ProfilePropertyKeys.items));

            profile.Add(new ObservableFloat(ProfilePropertyKeys.restoreHealthMultiplier));
            profile.Add(new ObservableFloat(ProfilePropertyKeys.restoreStaminaMultiplier));

            profile.Add(new ObservableDateTime(ProfilePropertyKeys.nextFreeMoneyReceiveTime, DateTime.MinValue));
        }
    }
}
