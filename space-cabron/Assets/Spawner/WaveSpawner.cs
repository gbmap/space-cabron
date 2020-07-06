﻿using Frictionless;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using Z;

public enum EWaveType
{
    CellBlock
}

[System.Serializable]
public class Wave
{
    public EWaveType type;
    public int enemies;
    
    [HideInInspector]
    public int enemiesSpawned;

    public int Index;
}

[System.Serializable]
public class WaveCellBlock : Wave
{
    public Vector2Int size;
    public bool[,] enemySpawns;
}

public class WaveGenerator
{
    private int WaveIndex;

    public WaveGenerator()
    {
    }

    public Wave GenerateRandomWave()
    {
        var values = System.Enum.GetValues(typeof(EWaveType));
        EWaveType waveType = (EWaveType)values.GetValue(Random.Range(0, values.Length));

        Wave w = new Wave();
        w.type = EWaveType.CellBlock;
        w.enemies = Random.Range(3, 5);
        w.Index = WaveIndex++;
        //w.enemiesSpawned = w.enemies;
        return w;
    }
}

public class WaveSpawner : MonoBehaviour
{
    public ZBaseMarchPlayer Beatmaker;
    public EnemySpawner spawner;

    private WaveGenerator gen;

    MessageRouter _router;

    private Wave CurrentWave
    {
        get; set; 
    }

    private int _currentEnemies;

    private void Awake()
    {
        gen = new WaveGenerator();
    }

    private void OnEnable()
    {
        Beatmaker.OnBar += OnBar;
        _router = ServiceFactory.Instance.Resolve<MessageRouter>();
    }

    private void OnDisable()
    {
        Beatmaker.OnBar -= OnBar;
    }

    private void OnBar(int obj)
    {
        if (CurrentWave == null)
        {
            // gerar nova wave
            CurrentWave = gen.GenerateRandomWave();
            Beatmaker.OnBeat += OnBeat;
        }
    }

    private void OnBeat(int b)
    {
        var w = CurrentWave;
        if (w == null) return;

        if (w.enemiesSpawned < w.enemies)
        {
            float t = (float)w.enemiesSpawned / w.enemies;
            float offset = 1f;
            float x0 = (offset * (w.enemies-1)) * -0.5f;
            float xo = x0 + offset * w.enemiesSpawned;

            Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.9f, 0f));
            Vector3 pos = new Vector3(xo, worldPos.y, 0f);

            var values = System.Enum.GetValues(typeof(EEnemyType));

            GameObject enemy = spawner.Spawn((EEnemyType)values.GetValue(Random.Range(0, values.Length)), pos);
            enemy.GetComponent<ObjectPool.ObjectPoolBehavior>().Destroyed += OnEnemyDestroyed;

            w.enemiesSpawned++;
            _currentEnemies++;
        }
        else
        {
            Beatmaker.OnBeat -= OnBeat;
        }
    }

    void OnEnemyDestroyed(GameObject objectDestroyed)
    {
        _currentEnemies--;
        objectDestroyed.GetComponent<ObjectPool.ObjectPoolBehavior>().Destroyed -= OnEnemyDestroyed;
        if (_currentEnemies == 0)
        {
            _router.RaiseMessage(new MsgOnWaveEnded
            {
                Wave = CurrentWave
            });
            CurrentWave = null;
        }
    }
}
