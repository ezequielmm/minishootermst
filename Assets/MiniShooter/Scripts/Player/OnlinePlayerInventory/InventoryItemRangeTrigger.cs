using UnityEngine;
using UnityEngine.Events;

namespace MiniShooter
{
    [RequireComponent(typeof(SphereCollider))]
    public class InventoryItemRangeTrigger : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        private InventoryItem inventoryItem;
        [SerializeField]
        private SphereCollider sphereCollider;

        public UnityEvent<PlayerCharacter> OnServerCharacterEnter;

        #endregion

        protected override void Awake()
        {
            base.Awake();

            sphereCollider.radius = 2f;
            sphereCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerCharacter playerCharacter))
            {
                if (isServer)
                    OnServerCharacterEnter?.Invoke(playerCharacter);
            }
        }
    }
}
