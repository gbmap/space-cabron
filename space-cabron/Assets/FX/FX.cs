using Frictionless;
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

    CameraShake _shake;

    System.Collections.Generic.Dictionary<EExplosionSize, GameObjectPool> _explosionPool;

    MessageRouter _router;

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

        _shake = FindObjectOfType<CameraShake>();

    }

    private void OnEnable()
    {
        _router = ServiceFactory.Instance.Resolve<MessageRouter>();
        _router.AddHandler<MsgOnEnemyHit>(Callback_OnEnemyHit);
        _router.AddHandler<MsgOnEnemyDestroyed>(Callback_OnEnemyDestroyed);
    }

    private void OnDisable()
    {
        _router.RemoveHandler<MsgOnEnemyHit>(Callback_OnEnemyHit);
        _router.RemoveHandler<MsgOnEnemyDestroyed>(Callback_OnEnemyDestroyed);
    }

    public void SpawnExplosion(EExplosionSize size, Vector3 pos)
    {
        _shake.Trauma += size == EExplosionSize.Big ? 0.2f : 0.1f;
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
            _shake.Trauma += sz == EExplosionSize.Big ? 0.2f : 0.1f;
            Vector3 offset = UnityEngine.Random.insideUnitSphere * 0.3f;
            offset.z = 0f;
            _explosionPool[sz].Instantiate(pos + offset, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Callback_OnEnemyHit(MsgOnEnemyHit obj)
    {
        SpawnExplosion(EExplosionSize.Medium, obj.bullet.transform.position);
    }

    private void Callback_OnEnemyDestroyed(MsgOnEnemyDestroyed obj)
    {
        SpawnExplosionCluster(4, obj.enemy.transform.position);
    }
}
