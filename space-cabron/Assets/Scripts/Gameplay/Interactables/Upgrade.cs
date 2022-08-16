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
        public static UpgradesContainer Upgrades = new UpgradesContainer();

        public List<UpgradePriceCategory> Price = new List<UpgradePriceCategory>();
        public override bool Interact(InteractArgs args)
        {
            bool hasCurrency = HasEnoughCurrency();
            if (hasCurrency)
            {
                Upgrades.Add(0, this);
                DeductFromCurrency(Price);
            }
            return hasCurrency;
        }

        protected bool HasEnoughCurrency()
        {
            var wallet = GetCurrency();
            return Price.All(p => wallet.ContainsKey(p.DroneType) 
                               && wallet[p.DroneType] >= p.Count);
        }

        private void DeductFromCurrency(List<UpgradePriceCategory> price)
        {
            price = price.Select(p => p.Clone()).ToList();
            foreach (GameObject drone in GameObject.FindGameObjectsWithTag("Drone"))
            {
                EDroneType droneType = GameObjectToDroneType(drone);
                if (price.FirstOrDefault(p => p.DroneType == droneType || p.DroneType == EDroneType.Any) == null)
                    continue;

                var priceCategory = price.First(p => p.DroneType == droneType || p.DroneType == EDroneType.Any);
                priceCategory.Count--;
                if (priceCategory.Count == 0)
                    price.Remove(priceCategory);

                Destroy(drone);
            }
        }

        protected Dictionary<EDroneType, int> GetCurrency()
        {
            Dictionary<EDroneType, int> currency = new Dictionary<EDroneType, int>();
            GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
            currency[EDroneType.Melody] = 0;
            currency[EDroneType.EveryN] = 0;
            currency[EDroneType.Random] = 0;
            currency[EDroneType.Any] = drones.Length; 

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

    [System.Serializable]
    public class UpgradesContainer
    {
        [SerializeField]
        List<List<Upgrade>> Upgrades = new List<List<Upgrade>>(4);

        public UpgradesContainer()
        {
            for (int i = 0; i < 4; i++)
                Upgrades.Add(new List<Upgrade>());
        }

        public List<Upgrade> GetUpgrades(int playerIndex)
        {
            if (Upgrades.Count <= playerIndex)
            {
                Debug.LogWarning($"No upgrades found for player {playerIndex}");
                return null;
            }
            if (Upgrades[playerIndex] == null)
                Upgrades[playerIndex] = new List<Upgrade>();
            return Upgrades[playerIndex];
        }

        public void Add(int playerIndex, Upgrade upgrade)
        {
            var upgrades = GetUpgrades(playerIndex);
            if (upgrades == null)
                return;
            upgrades.Add(upgrade);
        }

        public bool HasUpgrade(int playerIndex, Upgrade upgrade)
        {
            return GetUpgrades(playerIndex).Contains(upgrade);
        }

        ~UpgradesContainer()
        {
            Upgrades.Clear();
        }
    }
}