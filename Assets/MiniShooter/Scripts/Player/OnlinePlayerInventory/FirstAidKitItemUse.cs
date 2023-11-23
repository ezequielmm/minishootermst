using UnityEngine;

namespace MiniShooter
{
    public class FirstAidKitItemUse : InventoryItemUse
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private float healValue = 25f;

        #endregion

        protected override void Awake()
        {
            base.Awake();
        }

        public override bool Use(OnlinePlayer player)
        {
            if (player.TryGetComponent(out OnlinePlayerCharacter onlinePlayerCharacter)
                && onlinePlayerCharacter.Current
                && onlinePlayerCharacter.Current.TryGetComponent(out PlayerCharacterVitals vitals))
            {
                return vitals.Heal(healValue);
            }

            return false;
        }
    }
}