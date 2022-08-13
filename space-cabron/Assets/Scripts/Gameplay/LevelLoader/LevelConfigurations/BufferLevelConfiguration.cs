using System;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay.Interactables;
using UnityEngine;

namespace SpaceCabron.Gameplay.Level
{
    public class BufferLevelLoader : BaseLevelLoader<BufferLevelConfiguration>
    {
        public BufferLevelLoader(
            BufferLevelConfiguration levelConfiguration, 
            MonoBehaviour coroutineStarter, 
            string sceneName, 
            Action OnFinishedLoading
        )  : base(
            levelConfiguration, 
            coroutineStarter, 
            sceneName, 
            OnFinishedLoading
        ) {}

        protected override void ConfigureLevel()
        {
            base.ConfigureLevel();

            int numberOfUpgrades = levelConfiguration.NumberOfUpgrades.Value;
            float upgradeSpacing = 2f;
            float initialX = -(numberOfUpgrades * upgradeSpacing) / 2f;

            for (int i = 0; i < numberOfUpgrades; i++)
            {
                Interactable upgradeData = levelConfiguration.PossibleUpgrades.GetNext();

                var prefab = levelConfiguration.InteractablePrefab;
                var instance = GameObject.Instantiate(
                    prefab, 
                    new Vector3(initialX + i * upgradeSpacing, 0, 0), 
                    Quaternion.identity
                );
                instance.GetComponent<InteractableBehaviour>().Configure(upgradeData);
            }
        }
    }

    public class BufferLevelConfiguration : BaseLevelConfiguration
    {
        public IntReference NumberOfUpgrades;
        public GameObject InteractablePrefab;

        public InteractablePool PossibleUpgrades;
        public Interactable NextLevel;

        public override ILevelConfiguration Clone()
        {
            var cfg = ScriptableObject.CreateInstance<BufferLevelConfiguration>();
            cfg.NumberOfUpgrades = NumberOfUpgrades.Clone() as IntReference;
            cfg.InteractablePrefab = InteractablePrefab;
            cfg.PossibleUpgrades = PossibleUpgrades.Clone() as InteractablePool;
            return cfg;
        }

        public override ILevelLoader GetLoader(Action OnFinishedLoading = null)
        {
            return new BufferLevelLoader(
                this, 
                LevelLoader.CoroutineStarter, 
                "Gameplay", 
                OnFinishedLoading
            );
        }
    }
}