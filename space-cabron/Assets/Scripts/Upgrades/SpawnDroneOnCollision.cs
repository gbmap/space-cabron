using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class SpawnDroneOnCollision : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            MessageRouter.RaiseMessage(new Messages.MsgSpawnDrone { Player = collider.gameObject });
        }
    }
}