using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class SpawnDroneOnCollision : MonoBehaviour
    {
        public Messages.MsgSpawnDrone.EDroneType DroneType;
        private bool hasSpawned = false;
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (!hasSpawned)
            {
                MessageRouter.RaiseMessage(new Messages.MsgSpawnDrone 
                { 
                    DroneType = DroneType,
                    Player = collider.gameObject 
                });
            }
            hasSpawned = true;
        }
    }
}