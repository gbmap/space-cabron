using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class TemporaryInvincibility : MonoBehaviour
    {
        public float InvincibilityTime = 5f;
        Health health;
        new Renderer renderer;
        void Awake()
        {
            health = GetComponent<Health>();
            health.CanTakeDamage = false;
            renderer = GetComponent<Renderer>();
            StartCoroutine(Coroutine());
        }

        IEnumerator Coroutine()
        {
            float interval = InvincibilityTime;
            renderer.enabled = false;
            while (interval > 0.01f)
            {
                if (renderer)
                    renderer.enabled = !renderer.enabled;
                yield return new WaitForSeconds(interval/InvincibilityTime);
                interval -= interval/InvincibilityTime;
            }
            if (renderer)
                renderer.enabled = true;
            health.CanTakeDamage = true;
        }
    }
}