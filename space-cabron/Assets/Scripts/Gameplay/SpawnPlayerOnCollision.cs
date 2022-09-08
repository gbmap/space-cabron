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
            MessageRouter.RaiseMessage(new MsgSpawnPlayer 
            { 
                PlayerIndex = Mathf.Clamp(gameObject.name[gameObject.name.Length-1] - '0', 0, 1),
                Position = transform.position,
                IsRespawn = true
            });
            other.GetComponent<Health>().Destroy();
            health.Destroy();
        }
    }
}