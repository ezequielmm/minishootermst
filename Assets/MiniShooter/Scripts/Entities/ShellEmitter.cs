using UnityEngine;

namespace MiniShooter
{
    public class ShellEmitter : MonoBehaviour
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private float speed = 100f;
        [SerializeField]
        private float destroyafter = 1f;
        [SerializeField]
        private LayerMask layerMask;

        #endregion

        private Vector3 lastPosition;

        private void Start()
        {
            lastPosition = transform.position;
            Destroy(gameObject, destroyafter);
        }

        // Update is called once per frame
        void Update()
        {
            float deltaTime = Time.deltaTime;

            transform.Translate(Vector3.forward * deltaTime * speed);

            if (Physics.Raycast(lastPosition, transform.forward, deltaTime * speed, layerMask.value))
                Destroy(gameObject);

            lastPosition = transform.position;
        }
    }
}