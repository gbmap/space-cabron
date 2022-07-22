using Frictionless;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Utils;

[Serializable]
public class Lanes
{
    public int NLanes = 8;
    public int CurrentLane = 3;
    public float GetViewportX(int lane)
    {
        return ((float)lane) / NLanes;
    }

    public void OnSpawn()
    {
        CurrentLane += UnityEngine.Random.Range(0f, 1f) > 0.5 ? 1 : -1;
        CurrentLane = Math.Min(Mathf.Max(CurrentLane, 1), NLanes - 1);
    }

    public Vector3 GetSpawnPosition()
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(GetViewportX(CurrentLane), 1.1f, 0f));
    }
}


public class EnemySpawner : MonoBehaviour
{
    // DATA ===============
    public EnemyPrefabs EnemyPrefabs;
    Dictionary<GameObject, ObjectPool.GameObjectPool> _poolEnemies;

    // ============ LANE (controls spawn positions)
    public Lanes Lane;

    MessageRouter _router;

    void Start()
    {
        _poolEnemies = new Dictionary<GameObject, ObjectPool.GameObjectPool>();
        foreach (GameObject prefab in EnemyPrefabs.prefabs)
        {
            if (prefab == null) continue;

            _poolEnemies[prefab] = new ObjectPool.GameObjectPool(prefab);
            _poolEnemies[prefab].InitPool(50);
        }
    }

    public GameObject Spawn(EEnemyType type, Vector3 pos)
    {
        return _poolEnemies[EnemyPrefabs.GetPrefab(type)].Instantiate(pos, Quaternion.identity);
    }
}
