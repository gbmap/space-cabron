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
    public bool NeedsSpecialBullet;
    public SpawnOnDestroyItem[] items;

    public bool ApplyRigidbodyForce;
    public float Force = 2f;

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

    private void OnDestruction(MsgOnObjectDestroyed msg)
    {
        if (Random.value > Probability.Value)
            return;

        if (NeedsSpecialBullet && (msg.bullet == null || !msg.bullet.IsSpecial))
            return;

        var obj = Instantiate(bag.Next(), transform.position, Quaternion.identity);
        if (ApplyRigidbodyForce)
        {
            Rigidbody2D r = obj.GetComponent<Rigidbody2D>();
            if (r != null)
            {
                Vector2 force = Random.insideUnitCircle * Force;
                r.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}
