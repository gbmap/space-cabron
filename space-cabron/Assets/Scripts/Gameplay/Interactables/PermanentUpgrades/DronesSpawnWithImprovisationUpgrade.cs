using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{

    public class ImprovisationUpgrade : Upgrade 
    {
        public ScriptableImprovisation Improvisation;
    }

    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Upgrades/Drones Spawn With Improvisation")]
    public class DronesSpawnWithImprovisationUpgrade : ImprovisationUpgrade
    {
        // public ScriptableImprovisation Improvisation;

        public override bool Interact(InteractArgs args)
        {
            if (!base.Interact(args))
                return false;
            MessageRouter.AddHandler<MsgOnDroneSpawned>(Callback_SpawnDrone);
            return true;
        }
        
        void OnDestroy()
        {
            MessageRouter.RemoveHandler<MsgOnDroneSpawned>(Callback_SpawnDrone);
        }

        private void Callback_SpawnDrone(MsgOnDroneSpawned msg)
        {
            ITurntable turntable = msg.Drone.GetComponentInChildren<ITurntable>();
            if (turntable != null)
                turntable.ApplyImprovisation(Improvisation.Get(), false);
        }
    }
}