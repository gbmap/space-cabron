using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float Time = 3f;

        Health health;

        void Awake()
        {
            health = GetComponent<Health>();
        }

        void Start()
        {
            StartCoroutine(DestroyAfter(Time));
        }

        IEnumerator DestroyAfter(float time)
        {
            yield return new WaitForSeconds(time);

            if (health != null)
                health.Destroy();
            else
                Destroy(gameObject);
        }

    }
}