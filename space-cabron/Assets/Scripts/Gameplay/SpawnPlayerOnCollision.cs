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
            health.OnDestroy += Callback_OnDestroy;
        }

        private void Callback_OnDestroy(MsgOnObjectDestroyed obj)
        {
            MessageRouter.RaiseMessage(new MsgSpawnPlayer { Position = transform.position });
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Drone"))
                return;

            other.GetComponent<Health>().Destroy();
            health.Destroy();
        }
    }
}