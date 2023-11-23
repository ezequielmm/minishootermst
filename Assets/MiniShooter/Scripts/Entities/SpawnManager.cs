using MiniShooter;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniSurvival
{
    public class SpawnManager : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Settings"), SerializeField]
        private bool autoSpawn = true;
        [SerializeField, Range(1, 15)]
        private int maxInstances = 10;
        [SerializeField, Range(1f, 100f)]
        private int spawnRadius = 10;
        [SerializeField, Range(1f, 20f)]
        private int spawnHeight = 10;
        [SerializeField, Range(1f, 600f)]
        private int spawnInterval = 10;
        [SerializeField]
        private LayerMask spawnLayer;

        [Header("Components"), SerializeField]
        private NetworkIdentity objectPrefab;

        #endregion

        public List<NetworkIdentity> Instances { get; private set; } = new List<NetworkIdentity>();

        private bool spawnProcessIsRunning = false;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);
            UnityEditor.Handles.DrawWireDisc(transform.position + Vector3.up * spawnHeight, Vector3.up, 1f);
            UnityEditor.Handles.DrawLine(transform.position, transform.position + Vector3.up * spawnHeight);
            UnityEditor.Handles.DrawLine(transform.position + Vector3.up * spawnHeight, transform.position + Vector3.left * spawnRadius);
        }
#endif

        #region SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (autoSpawn)
            {
                Spawn();
                StartCoroutine(SpawnObjectsByInterval());
            }
        }

        private IEnumerator SpawnObjectsByInterval()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(spawnInterval);
                Spawn();
            }
        }

        private void RemoveDestroyed()
        {
            Instances = Instances.Where(i => i != null).ToList();
        }

        private IEnumerator StartSpawnProcess()
        {
            if (spawnProcessIsRunning) yield return null;

            RemoveDestroyed();

            if (Instances.Count < maxInstances && objectPrefab)
            {
                spawnProcessIsRunning = true;

                for (int i = Instances.Count; i < maxInstances; i++)
                {
                    yield return new WaitForSecondsRealtime(0.05f);

                    var randomCirclePosition = Random.insideUnitCircle;
                    var newPosition = new Vector3(randomCirclePosition.x * spawnRadius, 0f, randomCirclePosition.y * spawnRadius) + transform.position;

                    if (Physics.Raycast(newPosition + Vector3.up * spawnHeight, Vector3.down, out RaycastHit hitInfo, spawnHeight, spawnLayer.value))
                    {
                        newPosition = hitInfo.point;
                    }

                    var spawnedObject = Instantiate(objectPrefab, newPosition, Quaternion.identity);

                    Instances.Add(spawnedObject);
                    NetworkServer.Spawn(spawnedObject.gameObject);
                }

                spawnProcessIsRunning = false;
            }
        }

        public void Spawn()
        {
            StartCoroutine(StartSpawnProcess());
        }

        #endregion
    }
}