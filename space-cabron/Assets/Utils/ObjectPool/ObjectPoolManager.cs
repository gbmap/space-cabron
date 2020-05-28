using Frictionless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Useful;

namespace Managers
{
    class ObjectPoolManager : MonoBehaviour
    {
        Dictionary<string, ObjectPool.GameObjectPool> m_objectPools;

        void Awake()
        {
            m_objectPools = new Dictionary<string, ObjectPool.GameObjectPool>();

            /* Preload some stuff */
            var enemiesPrefabs = ResourceManager.GetScriptableObject<EnemyPrefabs>();
            GetPool(enemiesPrefabs.enemyBullet, 500);
            for (int i = 0; i < enemiesPrefabs.prefabs.Length; i++)
            {
                GetPool(enemiesPrefabs.prefabs[i], 25);
            }
        }

        void OnDestroy()
        {
            foreach (KeyValuePair<string, ObjectPool.GameObjectPool> kvp in m_objectPools)
                kvp.Value.DestroyPool();
        }

        internal bool InitPool(GameObject pPrefab, int pQuantity)
        {
            if (m_objectPools.ContainsKey(pPrefab.name))
            {
                Debug.Log("[ObjectPoolManager::InitPool] Pool for prefab {0} already exists.");
                return false;
            }

            ObjectPool.GameObjectPool pool = new ObjectPool.GameObjectPool(pPrefab);
            pool.InitPool(pQuantity);

            m_objectPools[pPrefab.name] = pool;
            return true;
        }

        internal ObjectPool.GameObjectPool GetPool(GameObject pPrefab, int pInitQuantity = 100)
        {
            if (!m_objectPools.ContainsKey(pPrefab.name))
            {
                Debug.Log("[ObjectPoolManager::GetPool] Pool requested not present. Initializing one.");
                if (!InitPool(pPrefab, pInitQuantity))
                {
                    Debug.Log("[ObjectPoolManager::GetPool] Couldn't initialize pool, returning null.");
                    return null;
                }
            }

            return m_objectPools[pPrefab.name];
        }
    }
}
