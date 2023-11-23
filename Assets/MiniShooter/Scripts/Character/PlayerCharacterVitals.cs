using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace MiniShooter
{
    public class PlayerCharacterVitals : NetworkEntityBehaviour, IDamageable, IHealable
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private PlayerCharacter playerCharacter;
        [SerializeField]
        private PlayerCharacterMovement playerCharacterMovement;

        [Header("Vitals"), SerializeField]
        private float healthRestore = 0.1f;
        [SerializeField]
        private float staminaRestore = 0.1f;
        [SerializeField]
        private float staminaReduce = 0.1f;

        public UnityEvent OnParamsChangeEvent;

        #endregion

        private ObservableFloat healthProperty;
        private ObservableFloat maxHealthProperty;
        private ObservableFloat restoreHealthMultiplierProperty;
        private ObservableFloat staminaProperty;
        private ObservableFloat maxStaminaProperty;
        private ObservableFloat restoreStaminaMultiplierProperty;
        private ObservableInt totalDeathsProperty;

        [SyncVar(hook = nameof(OnStaminaFrozen))]
        private bool isStaminaFrozen = false;

        [SyncVar(hook = nameof(OnHealthChanged))]
        private float health = 1f;

        [SyncVar(hook = nameof(OnMaxHealthChanged))]
        private float maxHealth = 100f;

        [SyncVar(hook = nameof(OnStaminaChanged))]
        private float stamina = 1f;

        [SyncVar(hook = nameof(OnMaxStaminaChanged))]
        private float maxStamina = 1f;

        [SyncVar]
        private bool isDead = false;

        [SyncVar]
        private bool isTired = false;

        protected RoomServerManager roomServerManager;
        protected RoomPlayer roomPlayer;
        protected NotificationRoomModule notificationRoomModule;

        public float Health => health;
        public float MaxHealth => maxHealth;
        public float Stamina => stamina;
        public float MaxStamina => maxStamina;
        public bool IsStaminaFrozen => isStaminaFrozen;

        #region SHARED

        private void Update()
        {
            if (isServer)
            {
                if (restoreHealthMultiplierProperty != null)
                    Heal(healthRestore * restoreHealthMultiplierProperty.Value * Time.deltaTime);

                if (playerCharacterMovement.IsRunning && !isStaminaFrozen)
                    ChangeStamina(-staminaReduce * Time.deltaTime);
                else
                    if (restoreStaminaMultiplierProperty != null)
                    ChangeStamina(staminaRestore * restoreStaminaMultiplierProperty.Value * Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (isServer)
            {
                if (healthProperty != null)
                    healthProperty.OnDirtyEvent -= CharacterHealth_OnDirtyEvent;

                if (staminaProperty != null)
                    staminaProperty.OnDirtyEvent -= CharacterStamina_OnDirtyEvent;
            }
        }

        #endregion

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();

            roomServerManager = FindObjectOfType<RoomServerManager>();
            notificationRoomModule = roomServerManager.GetComponentInChildren<NotificationRoomModule>();
            roomPlayer = roomServerManager.GetRoomPlayerByRoomPeer(connectionToClient.connectionId);

            // Init max health and stamina
            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.maxHealth, out maxHealthProperty))
                maxHealth = maxHealthProperty.Value;

            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.maxStamina, out maxStaminaProperty))
                maxStamina = maxStaminaProperty.Value;

            // Init restore value of health and stamina
            roomPlayer.Profile.TryGet(ProfilePropertyKeys.restoreHealthMultiplier, out restoreHealthMultiplierProperty);
            roomPlayer.Profile.TryGet(ProfilePropertyKeys.restoreStaminaMultiplier, out restoreStaminaMultiplierProperty);

            // Init health property
            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.health, out healthProperty))
            {
                healthProperty.OnDirtyEvent += CharacterHealth_OnDirtyEvent;

                if (healthProperty.Value <= 0f)
                    healthProperty.Value = maxHealthProperty.Value;

                healthProperty.MarkDirty();
            }

            // Init stamina property
            if (roomPlayer.Profile.TryGet(ProfilePropertyKeys.stamina, out staminaProperty))
            {
                staminaProperty.OnDirtyEvent += CharacterStamina_OnDirtyEvent;

                if (staminaProperty.Value <= 0f)
                    staminaProperty.Value = maxStaminaProperty.Value;

                staminaProperty.MarkDirty();
            }

            roomPlayer.Profile.TryGet(ProfilePropertyKeys.totalDeaths, out totalDeathsProperty);
        }

        private void CharacterStamina_OnDirtyEvent(IObservableProperty property)
        {
            stamina = staminaProperty.Value;
        }

        private void CharacterHealth_OnDirtyEvent(IObservableProperty property)
        {
            health = healthProperty.Value;
        }

        [Server]
        private void ChangeStamina(float value)
        {
            if (isDead) return;

            if (staminaProperty == null) return;

            staminaProperty.Value = Mathf.Clamp(staminaProperty.Value + value, 0f, maxStaminaProperty.Value);

            if (staminaProperty.Value <= 0.1f)
                isTired = true;

            if (isTired && staminaProperty.Value >= 10f)
                isTired = false;

            playerCharacterMovement.AllowRunning(staminaProperty.Value > 0f && !isTired);
        }

        /// <summary>
        /// Heals the character
        /// </summary>
        /// <param name="value"></param>
        [Server]
        public bool Heal(float value)
        {
            if (isDead) return false;
            if (healthProperty == null) return false;
            if (maxHealthProperty == null) return false;

            float oldValue = healthProperty.Value;
            healthProperty.Value = Mathf.Clamp(healthProperty.Value + value, 0f, maxHealthProperty.Value);

            if (health < healthProperty.Value)
                healthProperty.MarkDirty();

            return oldValue != healthProperty.Value;
        }

        /// <summary>
        /// Makes damage of the character
        /// </summary>
        /// <param name="value"></param>
        [Server]
        public void Damage(float value) => Damage(value, null);

        /// <summary>
        /// Makes damage of the character
        /// </summary>
        /// <param name="value"></param>
        /// <param name="damageGiver"></param>
        [Server]
        public void Damage(float value, IIdentifiable damageGiver)
        {
            if (healthProperty == null) return;

            float oldHealth = healthProperty.Value;

            healthProperty.Value = Mathf.Clamp(healthProperty.Value - value, 0f, maxHealthProperty.Value);

            if (healthProperty.Value < oldHealth)
                playerCharacter.ServerPlayerCharacter.NotifyCharacterTakeDamage();

            if (healthProperty.Value <= 0f && !isDead)
            {
                isDead = true;

                // Add death counter
                if (totalDeathsProperty != null)
                    totalDeathsProperty.Add(1);

                notificationRoomModule.NotifyPlayerDied(roomPlayer.RoomPeerId, damageGiver);
                playerCharacter.ServerPlayerCharacter.NotifyCharacterDied();

                // Destroy our character
                NetworkServer.Destroy(gameObject);
            }
        }

        /// <summary>
        /// Freezes the character's stamina for a while
        /// </summary>
        /// <returns></returns>
        [Server]
        public bool TryFreezeStamina()
        {
            if (!isStaminaFrozen)
            {
                isStaminaFrozen = true;

                MstTimer.WaitForSeconds(10f, () =>
                {
                    isStaminaFrozen = false;
                });

                return true;
            }

            return false;
        }

        #endregion

        #region CLIENT

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            MstTimer.WaitForSeconds(0.1f, () =>
            {
                OnParamsChangeEvent?.Invoke();
            });
        }

        private void OnStaminaFrozen(bool oldState, bool newState)
        {
            if (isOwned)
                OnParamsChangeEvent?.Invoke();
        }

        private void OnHealthChanged(float oldHealth, float newHalth)
        {
            if (isOwned)
                OnParamsChangeEvent?.Invoke();
        }

        private void OnMaxHealthChanged(float oldMaxHealth, float newMaxHalth)
        {
            if (isOwned)
                OnParamsChangeEvent?.Invoke();
        }

        private void OnStaminaChanged(float oldStamina, float newStamina)
        {
            if (isOwned)
                OnParamsChangeEvent?.Invoke();
        }

        private void OnMaxStaminaChanged(float oldMaxStamina, float newMaxStamina)
        {
            if (isOwned)
                OnParamsChangeEvent?.Invoke();
        }

        #endregion
    }
}