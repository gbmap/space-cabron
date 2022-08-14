using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SpaceCabron.Messages.MsgSpawnDrone;

namespace SpaceCabron.Gameplay.Interactables
{
    [System.Serializable]
    public class UpgradePriceCategory
    {
        public EDroneType DroneType;
        public int Count;

        public UpgradePriceCategory Clone()
        {
            return new UpgradePriceCategory
            {
                DroneType = DroneType,
                Count = Count
            };
        }
    }

    public abstract class Upgrade : Interactable 
    {
        public List<UpgradePriceCategory> Price;
        public override bool Interact(InteractArgs args)
        {
            bool hasCurrency = HasEnoughCurrency();
            if (hasCurrency)
                DeductFromCurrency(Price);
            return hasCurrency;
        }


        protected bool HasEnoughCurrency()
        {
            var wallet = GetCurrency();
            return Price.All(p => wallet[p.DroneType] >= p.Count);
        }

        private void DeductFromCurrency(List<UpgradePriceCategory> price)
        {
            price = price.Select(p => p.Clone()).ToList();
            while (price.Any(p => p.Count > 0))
            {
                GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
                GameObject drone = drones[0];
                EDroneType droneType = GameObjectToDroneType(drone);
                GameObject.Destroy(drone);
                price.First(p => p.DroneType == droneType).Count--;
            }
        }

        protected Dictionary<EDroneType, int> GetCurrency()
        {
            Dictionary<EDroneType, int> currency = new Dictionary<EDroneType, int>();
            GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
            for (int i = 0; i < drones.Length; i++)
            {
                var drone = drones[i];
                currency[GameObjectToDroneType(drone)] += 1;
            }
            return currency;
        }

        protected EDroneType GameObjectToDroneType(GameObject drone)
        {
            if (drone.name.Contains("Melody"))
                return EDroneType.Melody;
            else if (drone.name.Contains("EveryN"))
                return EDroneType.EveryN;
            else 
                return EDroneType.Random;
        }
    }
}