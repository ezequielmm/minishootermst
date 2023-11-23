using MasterServerToolkit.Bridges;
using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Utils;
using UnityEngine;

namespace MiniShooter
{
    public class RaycastWeapon : PlayerCharacterWeapon
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        protected LayerMask layerMask;

        [Header("Prefabs"), SerializeField]
        private PoolMonoBehaviour shellHitEffectPrefab;
        [SerializeField]
        private ParticleSystem shellEmitter;
        [SerializeField, Range(1, 10)]
        private int numberOfShells = 1;

        #endregion

        #region SHARED

        #endregion

        #region SERVER

        private void CheckRaycastOnServer()
        {
            if (shellSpawnPoint)
            {
                Ray ray = new Ray(shellSpawnPoint.position, shellSpawnPoint.forward);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask.value))
                {
                    if (hitInfo.collider.TryGetComponent(out IDamageable damageable) && damageable.Health > 0f)
                    {
                        // Do some damage
                        damageable.Damage(damage, this);

                        // Check if  health another damageable object is 0
                        if (damageable.Health <= 0f && roomPlayer.Profile.TryGet(ProfilePropertyKeys.totalKills, out ObservableInt totalKills))
                            totalKills.Add(1);
                    }
                }
            }
        }

        protected override void ShotOnServer()
        {
            base.ShotOnServer();
            CheckRaycastOnServer();
        }

        #endregion

        #region CLIENT

        public override void OnStartClient()
        {
            base.OnStartClient();
            RoomObjectsPoolManager.RegisterPrefab(shellHitEffectPrefab);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
        }

        private void CheckRaycastOnClient()
        {
            if (shellSpawnPoint)
            {
                Ray ray = new Ray(shellSpawnPoint.position, shellSpawnPoint.forward);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask.value))
                {
                    if (shellHitEffectPrefab
                        && RoomObjectsPoolManager.TryGet(shellHitEffectPrefab, out PoolMonoBehaviour hitEffectInstance))
                    {
                        hitEffectInstance.transform.SetPositionAndRotation(hitInfo.point, Quaternion.identity);

                        Tweener.DelayedCall(.25f, () =>
                        {
                            RoomObjectsPoolManager.Store(hitEffectInstance);
                        });
                    }
                }

                if (shellEmitter != null)
                    shellEmitter.Emit(numberOfShells);
            }
        }

        protected override void ShotOnClient()
        {
            base.ShotOnClient();
            CheckRaycastOnClient();
        }

        #endregion
    }
}