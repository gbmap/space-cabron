using System;
using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class SpawnPlayerOnCollision : MonoBehaviour
    {
        Health health;
        private bool hasSpawned;

        void Awake()
        {
            health = GetComponent<Health>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Drone"))
                return;
            
            if (hasSpawned)
                return;

            hasSpawned = true;
            MessageRouter.RaiseMessage(new MsgSpawnPlayer { Position = transform.position });
            other.GetComponent<Health>().Destroy();
            health.Destroy();
        }
    }
}