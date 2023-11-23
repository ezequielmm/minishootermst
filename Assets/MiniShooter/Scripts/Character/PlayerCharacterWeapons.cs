using MasterServerToolkit.UI;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniShooter
{
    public class PlayerCharacterWeapons : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private PlayerCharacterInput playerCharacterInput;

        private List<PlayerCharacterWeapon> weapons;

        public UnityEvent<PlayerCharacterWeapon> OnWeaponChangeEvent;
        public UnityEvent<PlayerCharacterWeapon> OnReloadEvent;

        #endregion

        private int weaponIndex = 0;

        public override bool IsReady => !ViewsManager.IsInputBlocked;

        /// <summary>
        /// Currently selected weapon
        /// </summary>
        public PlayerCharacterWeapon Weapon
        {
            get
            {
                if (weapons.Count > weaponIndex)
                    return weapons[weaponIndex];

                return null;
            }
        }

        #region SHARED

        protected override void Awake()
        {
            base.Awake();
            weapons = GetComponents<PlayerCharacterWeapon>().ToList();
        }

        private void Update()
        {
            if (isOwned && IsReady)
            {
                for (int i = 0; i < playerCharacterInput.NumberKeys.Length; i++)
                {
                    if (Input.GetKeyDown(playerCharacterInput.NumberKeys[i]))
                    {
                        ChangeWeapon(i);
                    }
                }

                if (playerCharacterInput.Fire() && playerCharacterInput.Armed())
                {
                    Weapon.Shoot();
                }

                // Reload weapon if is allowed
                if (playerCharacterInput.Reload() && Weapon.IsAllowedToReload)
                {
                    OnReloadEvent?.Invoke(Weapon);
                    Weapon.Reload();
                }
            }
        }

        private void UpdateWeaponVisibility()
        {
            foreach (var weapon in weapons)
                weapon.SetActive(false);

            Weapon.SetActive(true);
        }

        #endregion

        #region SERVER

        [Command]
        private void Cmd_ChangeWeapon(int index)
        {
            if (weapons.Count > index)
            {
                weaponIndex = index;
                UpdateWeaponVisibility();
                Rpc_ChangeWeapon(index);
            }
        }

        #endregion

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();

            UpdateWeaponVisibility();
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            OnWeaponChangeEvent?.Invoke(Weapon);
        }

        [ClientRpc]
        private void Rpc_ChangeWeapon(int index)
        {
            weaponIndex = index;

            if (!isOwned)
                UpdateWeaponVisibility();

            if (isOwned)
                OnWeaponChangeEvent?.Invoke(Weapon);
        }

        [Client]
        public void ChangeWeapon(int index)
        {
            if (isOwned && !Weapon.IsReloading && weaponIndex != index && weapons.Count > index)
            {
                weaponIndex = index;
                UpdateWeaponVisibility();
                Cmd_ChangeWeapon(index);
            }
        }

        #endregion
    }
}