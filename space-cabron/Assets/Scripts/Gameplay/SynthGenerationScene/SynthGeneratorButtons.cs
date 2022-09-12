using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;
using static SpaceCabron.Messages.MsgSpawnDrone;

public class SynthGeneratorButtons : MonoBehaviour
{
    public void SpawnDroneMelody() {
        MessageRouter.RaiseMessage(new MsgSpawnDrone{
            DroneType = EDroneType.Melody
        });
    }

    public void DestroyDroneMelody() {
        var drone = GameObject.FindGameObjectWithTag("Drone");
        if (drone != null)
            GameObject.Destroy(drone);
    }
}
