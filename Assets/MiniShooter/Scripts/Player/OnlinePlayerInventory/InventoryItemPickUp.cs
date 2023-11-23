using UnityEngine;

namespace MiniShooter
{
    public abstract class InventoryItemPickUp : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Components"), SerializeField]
        protected InventoryItem inventoryItem;

        #endregion

        Vector3 initialPosition;
        float time = 0f;

        protected void Start()
        {
            initialPosition = transform.position;
            time = Random.Range(0f, 1f);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, time * 360f, transform.rotation.eulerAngles.z);
        }

        private void Update()
        {
            time += Time.deltaTime / 10f;
            transform.position = new Vector3(initialPosition.x, initialPosition.y + Mathf.PingPong(time, 0.2f), initialPosition.z);
            transform.Rotate(Vector3.up * Time.deltaTime * 50f);
        }

        public abstract void PickUp(PlayerCharacter character);
    }
}