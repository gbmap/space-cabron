using Frictionless;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using UnityEngine;
using static SpaceCabron.Messages.MsgSpawnDrone;

namespace SpaceCabron.Gameplay.Interactables.Level
{
    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Upgrades/Exchange Drones")]
    public class ExchangeDronesUpgrade : Upgrade
    {
        [SerializeField] Sprite icon;
        public override Sprite Icon => icon; 

        public EDroneType DroneReward;

        public override bool Interact(InteractArgs args)
        {
            bool success = base.Interact(args);
            if (success)
            {
                MessageRouter.RaiseMessage(new MsgSpawnDrone
                {
                    DroneType = DroneReward,
                    Player = args.Interactor
                });
            }
            return success;
        }
    }
}