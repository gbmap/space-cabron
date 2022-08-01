using System.Collections;
using System.Collections.Generic;
using Frictionless;
using QFSW;
using QFSW.QC;
using UnityEngine;

public static class ConsoleCommands
{
    [Command]
    public static void SpawnDrone()
    {
        MessageRouter.RaiseMessage(new SpaceCabron.Messages.MsgSpawnDrone{ Player = GameObject.FindGameObjectWithTag("Player") });

    }
}
