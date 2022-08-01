using Gmap.ScriptableReferences;
using Gmap.Utils;
using Gmap.Gameplay;
using UnityEngine;

[System.Serializable]
public class SpawnOnDestroyItem
{
    public GameObject obj;
    public int Weight;
}

public class SpawnOnDestroy : MonoBehaviour
{
    public FloatBusReference Probability;
    public SpawnOnDestroyItem[] items;

    private ShuffleBag<GameObject> bag;

    Health health;

    void Awake()
    {
        health = GetComponentInChildren<Health>();
        bag = new ShuffleBag<GameObject>();
        System.Array.ForEach(items, item => bag.Add(item.obj, item.Weight));
    }

    private void OnEnable()
    {
        health.OnDestroy += OnDestruction;
    }

    private void OnDisable()
    {
        health.OnDestroy -= OnDestruction;
    }

    private void OnDestruction(MessageOnEnemyDestroyed msg)
    {
        if (Random.value > Probability.Value)
            return;
        Instantiate(bag.Next(), transform.position, Quaternion.identity);
    }
}
