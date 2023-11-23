using MasterServerToolkit.MasterServer;
using UnityEngine;

namespace MiniShooter
{
    [CreateAssetMenu(menuName = MstConstants.CreateMenu + "Mini shooter/Profiles Settings")]
    public class ProfilesSettings : ScriptableObject
    {
        #region INSPECTOR

        [Header("Items")]
        public StartupWeaponItemInfo[] weaponItems;
        public StartupGenericItemInfo[] genericItems;

        [Header("Health")]
        public float health = 100f;
        public float maxHealth = 100f;
        public float restoreHealthMultiplier = 1f;

        [Header("Stamina")]
        public float stamina = 100f;
        public float maxStamina = 100f;
        public float restoreStaminaMultiplier = 1f;

        [Header("Money")]
        public int startMoney = 5000;
        public int freeMoney = 2000;
        public uint freeMoneyRate = 60 * 60;

        #endregion

        private void OnValidate()
        {
            health = Mathf.Clamp(health, 1, float.MaxValue);
            stamina = Mathf.Clamp(stamina, 1, float.MaxValue);

            maxHealth = Mathf.Clamp(maxHealth, health, float.MaxValue);
            maxStamina = Mathf.Clamp(maxStamina, stamina, float.MaxValue);

            restoreHealthMultiplier = Mathf.Clamp(restoreHealthMultiplier, 1f, float.MaxValue);
            restoreStaminaMultiplier = Mathf.Clamp(restoreStaminaMultiplier, 1f, float.MaxValue);
        }
    }
}