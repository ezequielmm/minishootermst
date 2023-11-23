using MasterServerToolkit.Bridges;
using MasterServerToolkit.Utils;
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MiniShooter
{
    public class DamageArea : NetworkEntityBehaviour, IIdentifiable
    {
        #region INSPECTOR

        [Header("Info"), SerializeField]
        private string id = string.Empty;
        [SerializeField]
        private string title = string.Empty;

        [Header("Settings"), SerializeField]
        private float damageValue = 10f;
        [SerializeField]
        protected bool destroyAfterTrigger = true;
        [SerializeField]
        protected float destroyDelay = 0f;
        [SerializeField]
        protected bool usePersistantDamage = false;
        [SerializeField, Range(1f, 120f)]
        protected float persistantDamageRate = 1f;

        [Header("Components"), SerializeField]
        protected PoolMonoBehaviour[] spawnOnTrigger;

        public UnityEvent OnServerTriggerEnterEvent;
        public UnityEvent OnClientTriggerEnterEvent;

        #endregion

        private bool isDestroing = false;
        private float lastPersistantDamageTime = 0f;

        protected Collider collidedObject;
        protected IDamageable damageable;

        public string Id => id;
        public string Title => title;

        #region SERVER

        private void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                if (!usePersistantDamage)
                {
                    if (other.TryGetComponent(out damageable))
                    {
                        DisableAllColliders();

                        OnServerTrigger();
                        OnServerTriggerEnterEvent?.Invoke();
                        Rpc_OnTriggerEnter();

                        if (destroyAfterTrigger && !isDestroing)
                        {
                            isDestroing = true;
                            Tweener.DelayedCall(destroyDelay, () =>
                            {
                                NetworkServer.Destroy(gameObject);
                            });
                        }
                    }
                }
                else
                {
                    if (destroyAfterTrigger && !isDestroing)
                    {
                        isDestroing = true;
                        Tweener.DelayedCall(destroyDelay, () =>
                        {
                            NetworkServer.Destroy(gameObject);
                        });
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isServer)
            {
                if (usePersistantDamage && Time.timeSinceLevelLoad - lastPersistantDamageTime >= persistantDamageRate)
                {
                    lastPersistantDamageTime = Time.timeSinceLevelLoad;

                    if (other.TryGetComponent(out damageable))
                    {
                        OnServerTrigger();
                        OnServerTriggerEnterEvent?.Invoke();
                        Rpc_OnTriggerEnter();
                    }
                }
            }
        }

        private void DisableAllColliders()
        {
            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        [Server]
        protected virtual void OnServerTrigger()
        {
            damageable?.Damage(damageValue, this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
        }

        #endregion

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();

            foreach (var spawn in spawnOnTrigger)
            {
                RoomObjectsPoolManager.RegisterPrefab(spawn);
            }
        }

        [ClientRpc]
        protected virtual void Rpc_OnTriggerEnter()
        {
            OnClientTriggerEnterEvent?.Invoke();
            RunEffect();
        }

        public void RunEffect()
        {
            if (spawnOnTrigger != null)
            {
                foreach (var prefab in spawnOnTrigger.Where(i => i != null))
                {
                    if (RoomObjectsPoolManager.TryGet(prefab, out PoolMonoBehaviour spawnInstance))
                    {
                        spawnInstance.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

                        Tweener.DelayedCall(destroyDelay, () =>
                        {
                            RoomObjectsPoolManager.Store(spawnInstance);
                        });
                    }
                }
            }
        }

        #endregion
    }
}