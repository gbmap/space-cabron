using Frictionless;
using ObjectPool;
using System;
using System.Collections;
using UnityEngine;
using Useful;
using Gmap.Gameplay;

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
        MessageRouter.AddHandler<MsgOnObjectHit>(Callback_OnEnemyHit);
        MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnEnemyDestroyed);
    }

    private void OnDisable()
    {
        MessageRouter.RemoveHandler<MsgOnObjectHit>(Callback_OnEnemyHit);
        MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnEnemyDestroyed);
    }

    public void SpawnExplosion(EExplosionSize size, Vector3 pos, bool damage=false)
    {
        _shake.Trauma += size == EExplosionSize.Big ? 0.2f : 0.1f;
        GameObject explosion = _explosionPool[size].Instantiate(pos, Quaternion.identity);
        explosion.GetComponent<DestroyFunction>().DamageOnDestroy = damage;
    }

    public void SpawnExplosionCluster(int n, Vector3 pos, bool shouldDamage)
    {
        StartCoroutine(CExplosionCluster(n, pos, shouldDamage));
    }

    private IEnumerator CExplosionCluster(int n, Vector3 pos, bool shouldDamage)
    {
        var values = Enum.GetValues(typeof(EExplosionSize));
        for (int i = 0; i < n; i++)
        {
            EExplosionSize sz = (EExplosionSize)values.GetValue(i < n * 0.75 ? 1 : 0);
            _shake.Trauma += sz == EExplosionSize.Big ? 0.2f : 0.1f;
            Vector3 offset = UnityEngine.Random.insideUnitSphere * 0.3f;
            offset.z = 0f;
            SpawnExplosion(sz, pos + offset, shouldDamage);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Callback_OnEnemyHit(MsgOnObjectHit obj)
    {
        Vector3 position = obj.health.transform.position;
        if (obj.bullet != null)
            position = obj.bullet.transform.position;
        SpawnExplosion(EExplosionSize.Medium, position);
    }

    private void Callback_OnEnemyDestroyed(MsgOnObjectDestroyed obj)
    {
        bool shouldDamage = obj.health != null 
                         && obj.health.CompareTag("Enemy")
                         && obj.bullet != null;
        SpawnExplosionCluster(4, obj.health.transform.position, shouldDamage);
    }
}
