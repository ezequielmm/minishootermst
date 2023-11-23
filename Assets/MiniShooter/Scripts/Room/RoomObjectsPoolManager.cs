using MasterServerToolkit.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace MiniShooter
{
    public class RoomObjectsPoolManager : DynamicSingletonBehaviour<RoomObjectsPoolManager>
    {
        private readonly Dictionary<int, int> instancesInUse = new Dictionary<int, int>();
        private readonly Dictionary<int, GenericPool<PoolMonoBehaviour>> pool = new Dictionary<int, GenericPool<PoolMonoBehaviour>>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        public static void RegisterPrefab<T>(T prefab) where T : PoolMonoBehaviour
        {
            if (TryGetOrCreate(out var poolInstance))
            {
                int id = prefab.GetInstanceID();

                if (!poolInstance.pool.ContainsKey(id))
                {
                    poolInstance.pool[id] = new GenericPool<PoolMonoBehaviour>(prefab, true);
                    poolInstance.logger.Info($"Registered prefab {id}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        public static T Get<T>(T prefab) where T : MonoBehaviour
        {
            if (TryGetOrCreate(out var poolInstance))
            {
                int id = prefab.GetInstanceID();

                if (poolInstance.pool.ContainsKey(id))
                {
                    T instanceInUse = poolInstance.pool[id].Get(true) as T;
                    int instanceInUseId = instanceInUse.GetInstanceID();
                    poolInstance.instancesInUse[instanceInUseId] = id;

                    poolInstance.logger.Info($"Use instance {instanceInUseId} from pool {id}");

                    return instanceInUse;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryGet<T>(T prefab, out T instance) where T : MonoBehaviour
        {
            instance = Get(prefab);
            return instance != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void Store<T>(T instance) where T : PoolMonoBehaviour
        {
            if (TryGetOrCreate(out var poolInstance))
            {
                int id = instance.GetInstanceID();

                poolInstance.logger.Info($"Trying to store instance {id}");

                if (poolInstance.instancesInUse.ContainsKey(id))
                {
                    int poolId = poolInstance.instancesInUse[id];
                    poolInstance.pool[poolId].Store(instance);
                    poolInstance.logger.Info($"Instance {id} stored in pool {poolId}");
                }
            }
        }
    }
}