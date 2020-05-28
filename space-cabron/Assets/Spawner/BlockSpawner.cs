using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Range(0f, 1f)]
    public float SpawnChance = 0.75f;
    public GameObject BlockPrefab;

    public TurnTable Turntable;
    public SpawnerTypeShuffle SpawnerType;

    ObjectPool.GameObjectPool _pool;

    private void Start()
    {
        _pool = new ObjectPool.GameObjectPool(BlockPrefab);
        _pool.InitPool(250);

        Turntable.Run();
    }

    private void OnEnable()
    {
        Turntable.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        Turntable.OnBeat -= OnBeat;
    }

    private void OnBeat(EInstrumentAudio obj)
    {
        if (obj == EInstrumentAudio.None) return;
        if (SpawnerType.Type != ESpawnerType.Block) return;

        var blockSprite = BlockPrefab.GetComponent<SpriteRenderer>().sprite;
        float blockW = blockSprite.bounds.size.x * 2f * blockSprite.pixelsPerUnit;
        float nBlocks = blockW / Screen.width;

        float y = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1.1f)).y;

        for (float x = 0f; x < Screen.width; x+= blockW)
        {
            float xx = Camera.main.ScreenToWorldPoint(Vector3.right * x).x;
            bool spawn = Mathf.PerlinNoise(xx, Time.time) > 1f - SpawnChance;
            if (spawn)
            {
                _pool.Instantiate(new Vector3(xx, y, 0f), Quaternion.identity);
            }
        }

        
    }
}
