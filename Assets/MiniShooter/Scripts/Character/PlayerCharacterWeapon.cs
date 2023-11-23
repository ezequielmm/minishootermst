using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MiniShooter
{
    public abstract class PlayerCharacterWeapon : NetworkEntityBehaviour, IIdentifiable
    {
        #region INSPECTOR

        [Header("Base Settings"), SerializeField]
        private InventoryItem itemInfo;
        [SerializeField]
        private int maxAmmoInClip = 10;
        [SerializeField]
        private float shootRate = 1f;
        [SerializeField]
        private float reloadTime = 2f;
        [SerializeField]
        protected float damage = 10f;

        [Header("Camera Settings"), SerializeField, Range(5f, 100f)]
        private float minDistance = 5f;
        [SerializeField, Range(5f, 100f)]
        private float maxDistance = 15f;
        [SerializeField, Range(5f, 100f)]
        private float startDistance = 15f;
        [SerializeField]
        private bool useDistanceChange = true;

        [Header("Audio"), SerializeField]
        private AudioClip shotSound;
        [SerializeField]
        private AudioClip reloadSound;
        [SerializeField]
        private AudioSource audioSource;

        [Header("Components"), SerializeField]
        protected Transform shellSpawnPoint;
        [SerializeField]
        private GameObject view;
        [SerializeField]
        private PlayerCharacterLook playerCharacterLook;

        public UnityEvent OnShotEvent;
        public UnityEvent OnReloadEvent;

        #endregion

        [SyncVar]
        protected int currentAmmo = 0;
        [SyncVar]
        protected int totalAmmo = 0;
        [SyncVar]
        private bool isNowReloading = false;

        private float lastShotTime = 0f;

        protected RoomServerManager roomServerManager;
        protected RoomPlayer roomPlayer;
        protected ObservableWeapons characterWeapons;

        /// <summary>
        /// 
        /// </summary>
        public string Id => itemInfo.ItemId;

        /// <summary>
        /// 
        /// </summary>
        public string Title => itemInfo.ItemTitle;

        /// <summary>
        /// 
        /// </summary>
        public Sprite Icon => itemInfo.ItemIcon;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReloading => isNowReloading;

        /// <summary>
        /// 
        /// </summary>
        public float ReloadTime => reloadTime;

        /// <summary>
        /// 
        /// </summary>
        public int CurrentAmmo => currentAmmo;

        /// <summary>
        /// 
        /// </summary>
        public int MaxInClip => maxAmmoInClip;

        /// <summary>
        /// 
        /// </summary>
        public int TotalAmmo => totalAmmo;

        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowedToReload => !isNowReloading && maxAmmoInClip > currentAmmo && totalAmmo > 0;

        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowedToShoot => currentAmmo > 0 && !isNowReloading && Time.time - lastShotTime > shootRate;

        /// <summary>
        /// 
        /// </summary>
        public Transform ShellSpawnPoint => shellSpawnPoint;

        #region SHARED

        private void PlayShotSound()
        {
            if (audioSource && shotSound)
                audioSource.PlayOneShot(shotSound);
        }

        private void PlayReloadSound()
        {
            if (audioSource && reloadSound)
                audioSource.PlayOneShot(reloadSound);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetActive(bool value)
        {
            if (view)
                view.SetActive(value);

            if (playerCharacterLook)
                playerCharacterLook.SetDistance(minDistance, maxDistance, startDistance);
        }

        #endregion

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();

            roomServerManager = FindObjectOfType<RoomServerManager>();
            roomPlayer = roomServerManager.GetRoomPlayerByRoomPeer(connectionToClient.connectionId);

            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.weapons, out characterWeapons))
            {
                if (characterWeapons.ContainsKey(itemInfo.ItemId))
                {
                    var weapon = characterWeapons[itemInfo.ItemId];
                    currentAmmo = weapon.CurrentAmmo;
                    totalAmmo = weapon.TotalAmmo;
                }

                characterWeapons.OnDirtyEvent += CharacterWeapons_OnDirtyEvent;
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (characterWeapons != null)
                characterWeapons.OnDirtyEvent -= CharacterWeapons_OnDirtyEvent;
        }

        private void CharacterWeapons_OnDirtyEvent(IObservableProperty property)
        {
            if (characterWeapons.ContainsKey(itemInfo.ItemId))
            {
                var weapon = property.As<ObservableWeapons>()[itemInfo.ItemId];
                currentAmmo = weapon.CurrentAmmo;
                totalAmmo = weapon.TotalAmmo;
            }
        }

        [Command]
        private void Cmd_Shoot()
        {
            if (characterWeapons != null && characterWeapons.ContainsKey(itemInfo.ItemId))
            {
                var weapon = characterWeapons[itemInfo.ItemId];
                weapon.CurrentAmmo = Mathf.Clamp(weapon.CurrentAmmo - 1, 0, maxAmmoInClip);
                characterWeapons[itemInfo.ItemId] = weapon;
            }

            ShotOnServer();
            Rpc_Shoot();
        }

        [Command]
        private void Cmd_Reload()
        {
            Rpc_Reload();
            StartCoroutine(ReloadCoroutine());
        }

        private IEnumerator ReloadCoroutine()
        {
            isNowReloading = true;

            if (characterWeapons != null && characterWeapons.ContainsKey(itemInfo.ItemId))
            {
                yield return new WaitForSecondsRealtime(reloadTime);

                var weapon = characterWeapons[itemInfo.ItemId];
                int totalToReload = maxAmmoInClip - weapon.CurrentAmmo;

                if (weapon.TotalAmmo > totalToReload)
                {
                    weapon.CurrentAmmo += totalToReload;
                    weapon.TotalAmmo -= totalToReload;
                }
                else
                {
                    weapon.CurrentAmmo += weapon.TotalAmmo;
                    weapon.TotalAmmo = 0;
                }

                characterWeapons[itemInfo.ItemId] = weapon;
            }
            else
            {
                yield return null;
            }

            isNowReloading = false;
        }

        protected virtual void ShotOnServer() { }

        #endregion

        #region CLIENT

        [ClientRpc(includeOwner = false)]
        private void Rpc_Shoot()
        {
            PlayShotSound();
            ShotOnClient();
            OnShotEvent?.Invoke();
        }

        [ClientRpc(includeOwner = false)]
        private void Rpc_Reload()
        {
            PlayReloadSound();
            OnReloadEvent?.Invoke();
        }

        protected virtual void ShotOnClient() { }

        /// <summary>
        /// 
        /// </summary>
        [Client]
        public void Shoot()
        {
            if (isOwned && IsAllowedToShoot)
            {
                lastShotTime = Time.time;

                PlayShotSound();
                ShotOnClient();
                Cmd_Shoot();
                OnShotEvent?.Invoke();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Client]
        public void Reload()
        {
            if (isOwned && IsAllowedToReload)
            {
                isNowReloading = true;

                PlayReloadSound();
                Cmd_Reload();
                OnReloadEvent?.Invoke();
            }
        }

        #endregion
    }
}