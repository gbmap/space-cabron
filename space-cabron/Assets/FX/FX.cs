using ObjectPool;
using System;
using System.Collections;
using UnityEngine;
using Useful;

public class FX : Singleton<FX>
{
    public enum EExplosionSize
    {
        Medium,
        Big
    }

    public FXPrefabs Prefabs;

    System.Collections.Generic.Dictionary<EExplosionSize, GameObjectPool> _explosionPool;

    void Awake()
    {
        var values = Enum.GetValues(typeof(EExplosionSize));
        _explosionPool = new System.Collections.Generic.Dictionary<EExplosionSize, GameObjectPool>();
        for (int i = 0; i < values.Length; i++)
        {
            EExplosionSize sz = (EExplosionSize)values.GetValue(i);
            _explosionPool[sz] = new GameObjectPool(Prefabs.Explosions[i]);
            _explosionPool[sz].InitPool(100);
        }
    }

    public void SpawnExplosion(EExplosionSize size, Vector3 pos)
    {
        _explosionPool[size].Instantiate(pos, Quaternion.identity);
    }

    public void SpawnExplosionCluster(int n, Vector3 pos)
    {
        StartCoroutine(CExplosionCluster(n, pos));
    }

    private IEnumerator CExplosionCluster(int n, Vector3 pos)
    {
        var values = Enum.GetValues(typeof(EExplosionSize));
        for (int i = 0; i < n; i++)
        {
            EExplosionSize sz = (EExplosionSize)values.GetValue(i < n * 0.75 ? 1 : 0);
            Vector3 offset = UnityEngine.Random.insideUnitSphere * 0.3f;
            offset.z = 0f;
            _explosionPool[sz].Instantiate(pos + offset, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }

}
