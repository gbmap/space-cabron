using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESpawnerType
{
    Block,
    Enemy,
    Charge
}

public class SpawnerTypeShuffle : MonoBehaviour
{
    public TurnTable TurnTable;
    public ESpawnerType Type;

    private void OnEnable()
    {
        TurnTable.OnBar += OnBar;
    }

    private void OnDisable()
    {
        TurnTable.OnBar -= OnBar;
    }

    private void OnBar(int obj)
    {
        Type = (ESpawnerType)UnityEngine.Random.Range(0, 2);
    }
}
