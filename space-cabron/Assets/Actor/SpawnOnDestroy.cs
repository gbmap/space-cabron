using UnityEngine;

[System.Serializable]
public struct SpawnOnDestroyItem
{
    public GameObject obj;
    public float chance;
}

public class SpawnOnDestroy : MonoBehaviour
{
    public SpawnOnDestroyItem[] items;

    Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDestroy += OnDestruction;
    }

    private void OnDisable()
    {
        health.OnDestroy -= OnDestruction;
    }

    private void OnDestruction()
    {
        foreach (var item in items)
        {
            if (Random.Range(0f, 1f) < item.chance)
            {
                Instantiate(item.obj, transform.position, Quaternion.identity);
                return;
            }
        }
    }
}
