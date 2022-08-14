using Gmap.ScriptableReferences;
using UnityEngine;
using static SpaceCabron.Messages.MsgSpawnDrone;

namespace SpaceCabron.Gameplay.Interactables.Level
{
    [CreateAssetMenu(menuName="Space Cabrón/Interactables/Upgrades/Exchange Drones")]
    public class ExchangeDronesUpgrade : Upgrade
    {
        public IntBusReference Cost;
        public EDroneType CostType;

        public IntBusReference RewardAmount;
        public EDroneType RewardType;

        public override bool Interact(InteractArgs args)
        {
            return base.Interact(args);
        }
    }
}