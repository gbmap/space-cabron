using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron
{
    public class DestroyOnCollision : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D other)
        {
            Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Destroy(gameObject);
        }
    }
}