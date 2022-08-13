using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    [CreateAssetMenu(menuName="Space Cabrón/Upgrades/Drones Spawn With Improvisation")]
    public class DronesSpawnWithImprovisationUpgrade : Upgrade
    {
        public ScriptableImprovisation Improvisation;

        public override void Interact(InteractArgs args)
        {
            base.Interact(args);
            MessageRouter.AddHandler<MsgOnDroneSpawned>(Callback_SpawnDrone);
        }
        
        void OnDestroy()
        {
            MessageRouter.RemoveHandler<MsgOnDroneSpawned>(Callback_SpawnDrone);
        }

        private void Callback_SpawnDrone(MsgOnDroneSpawned msg)
        {
            ITurntable turntable = msg.Drone.GetComponentInChildren<ITurntable>();
            turntable.ApplyImprovisation(Improvisation.Get(), false);
        }
    }
}