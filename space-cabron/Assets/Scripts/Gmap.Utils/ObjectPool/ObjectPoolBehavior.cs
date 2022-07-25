using System;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void DestroyOrDisable(this MonoBehaviour behaviour)
    {
        if (behaviour.GetComponent<ObjectPool.ObjectPoolBehavior>())
        {
            behaviour.gameObject.SetActive(false);
        }
        else
        {
            GameObject.Destroy(behaviour.gameObject);
        }
    }
}

namespace ObjectPool
{
    public class ObjectPoolBehavior : MonoBehaviour {
        public Action<GameObject> Destroyed;

        internal virtual void OnDisable() {
            Destroyed?.Invoke(gameObject);
        }
    }
}
