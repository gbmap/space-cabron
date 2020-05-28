using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    ObjectPool.ObjectPoolBehavior _poolBehavior;

    private void Awake()
    {
        _poolBehavior = GetComponent<ObjectPool.ObjectPoolBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var health = collision.GetComponent<Health>();
        if (health)
        {
            health.TakeDamage();
        }

        if (_poolBehavior)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
