using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class TakeDamageOnCollision : MonoBehaviour
    {
        Health health;

        void Awake()
        {
            health = GetComponent<Health>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {

        }

    }
}