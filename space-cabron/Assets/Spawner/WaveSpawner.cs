using Managers;
using System.Collections.Generic;
using UnityEngine;

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
}

[System.Serializable]
public class WaveCellBlock : Wave
{
    public Vector2Int size;
    public bool[,] enemySpawns;
}

public class WaveGenerator
{
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
        //w.enemiesSpawned = w.enemies;
        return w;
    }

}


public class WaveSpawner : MonoBehaviour
{
    public BeatMakerBehaviour BeatMakerBehaviour;
    public BeatMaker Beatmaker { get { return BeatMakerBehaviour.BeatMaker; } }
    public EnemySpawner spawner;

    private WaveGenerator gen;

    private Queue<Wave> _waveQueue;
    private Wave CurrentWave
    {
        get { return _waveQueue.Count > 0 ? _waveQueue.Peek() : null; }
    }

    int _currentEnemies;

    private void Awake()
    {
        gen = new WaveGenerator();
        _waveQueue = new Queue<Wave>();
    }

    private void OnEnable()
    {
        Beatmaker.OnBar += OnBar;
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
            _waveQueue.Enqueue(gen.GenerateRandomWave());
            Beatmaker.OnBeat += OnBeat;
        }
    }

    private void OnBeat(int[] obj)
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

            GameObject enemy = spawner.Spawn(EEnemyType.Follow, pos);
            enemy.GetComponent<Health>().OnDestroy += OnEnemyDestroyed;

            w.enemiesSpawned++;
            _currentEnemies++;
        }
        else
        {
        
            Beatmaker.OnBeat -= OnBeat;
        }
    }

    void OnEnemyDestroyed(Health objectDestroyed)
    {
        _currentEnemies--;
        if (_currentEnemies <= 0)
        {
            _waveQueue.Dequeue();
        }
    }
}
