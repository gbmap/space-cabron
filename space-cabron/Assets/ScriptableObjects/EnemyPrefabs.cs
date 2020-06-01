using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyType
{
    Cannon,
    Dasher,
    Follow,
    Mine,
    Thug,
    Test
}

[System.Serializable]
public class EnemyPrefabs : ScriptableObject
{
    public GameObject enemyBullet;
    public GameObject shieldCharge;
    public GameObject hackCharge;
    public GameObject shuffleCharge;
    
    public Material enemyDefaultMat;
    public Material enemyResistantMat;
    public GameObject[] prefabs;

    public GameObject GetPrefab(EEnemyType t)
    {
        return prefabs[(int)t];
    }
}
