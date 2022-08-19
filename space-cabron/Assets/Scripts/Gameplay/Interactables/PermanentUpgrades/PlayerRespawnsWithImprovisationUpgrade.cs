using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Upgrades/Player Spawns With Improvisation")]
    public class PlayerRespawnsWithImprovisationUpgrade : ImprovisationUpgrade
    {
        public override bool Interact(InteractArgs args)
        {
            if (!base.Interact(args))
                return false;

            TurntableBehaviour t = args.Interactor.GetComponentInChildren<TurntableBehaviour>();
            t.ApplyImprovisation(Improvisation.Get(), -1);

            MessageRouter.AddHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
            return true;
        }

        private void Callback_OnPlayerSpawned(MsgOnPlayerSpawned msg)
        {
            ITurntable turntable = msg.Player.GetComponentInChildren<ITurntable>();
            if (turntable != null)
                turntable.ApplyImprovisation(Improvisation.Get(), -1);
        }
    }
}