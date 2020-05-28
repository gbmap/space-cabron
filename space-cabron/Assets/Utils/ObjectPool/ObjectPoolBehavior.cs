using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ObjectPool {
    public class ObjectPoolBehavior : MonoBehaviour {
        internal event Action<GameObject> Destroyed;

        internal void OnDisable() {
            Destroyed?.Invoke(gameObject);
        }
    }
}
