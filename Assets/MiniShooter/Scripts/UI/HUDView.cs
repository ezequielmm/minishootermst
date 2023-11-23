using MasterServerToolkit.MasterServer;
using MasterServerToolkit.UI;
using UnityEngine;

namespace MiniShooter
{
    public class HUDView : UIView
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private WeaponUI weaponUI;

        #endregion

        private PlayerCharacterWeapons weapons;

        protected override void Awake()
        {
            base.Awake();

            OnlinePlayerCharacter.OnLocalCharacterCreatedEvent += OnlinePlayerCharacter_OnLocalCharacterCreatedEvent;
            OnlinePlayerCharacter.OnLocalCharacterDestroyedEvent += OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent;
        }

        protected override void OnDestroy()
        {
            OnlinePlayerCharacter.OnLocalCharacterCreatedEvent -= OnlinePlayerCharacter_OnLocalCharacterCreatedEvent;
            OnlinePlayerCharacter.OnLocalCharacterDestroyedEvent -= OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent;
        }

        private void OnlinePlayerCharacter_OnLocalCharacterCreatedEvent(PlayerCharacter playerCharacter)
        {
            if (playerCharacter.TryGetComponent(out weapons))
            {
                weapons.OnWeaponChangeEvent.AddListener(weaponUI.ChangeWeapon);
                weapons.OnReloadEvent.AddListener(weaponUI.StartReloadingProgress);
            }

            Show();
        }

        private void OnlinePlayerCharacter_OnLocalCharacterDestroyedEvent()
        {
            Hide();
        }

        /// <summary>
        /// Starts/Stops playing of ambient music
        /// </summary>
        /// <param name="isOn"></param>
        public void PlayMusic(bool isOn)
        {
            Mst.Events.Invoke(GameEventKeys.playAmbienceMusic, isOn);
        }
    }
}