using MasterServerToolkit.Extensions;

namespace MiniShooter
{
    public struct ProfilePropertyKeys
    {
        public static ushort displayName = nameof(displayName).ToUint16Hash();
        public static ushort avatarUrl = nameof(avatarUrl).ToUint16Hash();
        public static ushort totalKills = nameof(totalKills).ToUint16Hash();
        public static ushort totalDeaths = nameof(totalDeaths).ToUint16Hash();
        public static ushort money = nameof(money).ToUint16Hash();
        public static ushort health = nameof(health).ToUint16Hash();
        public static ushort maxHealth = nameof(maxHealth).ToUint16Hash();
        public static ushort stamina = nameof(stamina).ToUint16Hash();
        public static ushort maxStamina = nameof(maxStamina).ToUint16Hash();
        public static ushort characterEditor = nameof(characterEditor).ToUint16Hash();
        public static ushort items = nameof(items).ToUint16Hash();
        public static ushort weapons = nameof(weapons).ToUint16Hash();
        public static ushort restoreHealthMultiplier = nameof(restoreHealthMultiplier).ToUint16Hash();
        public static ushort restoreStaminaMultiplier = nameof(restoreStaminaMultiplier).ToUint16Hash();
        public static ushort nextFreeMoneyReceiveTime = nameof(nextFreeMoneyReceiveTime).ToUint16Hash();
    }
}