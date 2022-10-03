using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Multiplayer;
using SpaceCabron.Messages;
using UnityEngine;

public class AddRandomImprovisationOnComboThreshold : MonoBehaviour
{
    public ImprovisationPool ImprovisationPool;

    void OnEnable() {
        MessageRouter.AddHandler<MsgOnComboIncrease>(Callback_OnComboIncrease);
        MessageRouter.AddHandler<MsgOnComboBroken>(Callback_OnComboBroken);
    }

    void OnDisable() {
        MessageRouter.RemoveHandler<MsgOnComboIncrease>(Callback_OnComboIncrease);
        MessageRouter.AddHandler<MsgOnComboBroken>(Callback_OnComboBroken);
    }

    private void Callback_OnComboIncrease(MsgOnComboIncrease msg)
    {
        if (msg.CurrentCombo % 50 != 0 || msg.CurrentCombo == 0)
            return;

        if (msg.CurrentCombo < 500) {
            var turntable = TurntableResolver.Create("GlobalInstruments", "PlayerInstrument").Get();
            turntable.ApplyImprovisation(ImprovisationPool.GetNext().Get(), false);
        }

        if (GameObject.FindGameObjectsWithTag("Drone").Length < 5) {
            MessageRouter.RaiseMessage(
                new MsgSpawnDrone
                {
                    DroneType = MsgSpawnDrone.EDroneType.Melody,
                    Player = MultiplayerManager.GetPlayerWithIndex(msg.PlayerIndex)
                }
            );
        }
    }

    private void Callback_OnComboBroken(MsgOnComboBroken obj)
    {
        var turntable = TurntableResolver.Create("GlobalInstruments", "PlayerInstrument").Get();
        turntable.Improviser.Clear();
    }

}
