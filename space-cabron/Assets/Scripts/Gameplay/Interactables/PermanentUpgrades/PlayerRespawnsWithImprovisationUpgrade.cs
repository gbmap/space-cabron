using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    public class MsgOnPermanentImprovisationUpgrade
    {
        public GameObject Player;
        public int playerIndex = 0;
        public Improvisation Improvisation;
    }

    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Upgrades/Player Spawns With Improvisation")]
    public class PlayerRespawnsWithImprovisationUpgrade : ImprovisationUpgrade
    {
        public override bool Interact(InteractArgs args)
        {
            if (!base.Interact(args))
                return false;

            TurntableBehaviour t = args.Interactor.GetComponentInChildren<TurntableBehaviour>();
            t.ApplyImprovisation(Improvisation.Get(), -1);

            MessageRouter.RaiseMessage(
                new MsgOnPermanentImprovisationUpgrade {
                    Improvisation = Improvisation.Get(),
                    Player = args.Interactor
                }
            );
            return true;
        }
    }
}