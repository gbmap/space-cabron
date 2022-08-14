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

            int numberOfUpgrades = levelConfiguration.GetNumberOfUpgrades();
            float upgradeSpacing = 2f;
            float initialX = -(numberOfUpgrades * upgradeSpacing) / 2f;

            var interactablePrefab = Resources.Load<GameObject>("Interactable");
            for (int i = 0; i < numberOfUpgrades; i++)
            {
                Interactable.CreateInteractable(
                    levelConfiguration.PossibleUpgrades.GetNext(),
                    new Vector3(initialX + i * upgradeSpacing, 2f, 0)
                );
            }

            Interactable.CreateInteractable(
                levelConfiguration.NextLevelInteractable
            );
        }

        public override void Unload()
        {
            InteractableBehaviour[] interactables = GameObject.FindObjectsOfType<InteractableBehaviour>();
            System.Array.ForEach(
                interactables, 
                interactable => GameObject.Destroy(interactable.gameObject)
            );
        }
    }

    [CreateAssetMenu(menuName="Space Cabrón/Level/Buffer")]
    public class BufferLevelConfiguration : BaseLevelConfiguration
    {
        public IntReference NumberOfUpgrades;
        public InteractablePool PossibleUpgrades;
        public Interactable NextLevelInteractable;

        public int GetNumberOfUpgrades()
        {
            return Mathf.Min(NumberOfUpgrades.Value, PossibleUpgrades.Length);
        }

        public override ILevelConfiguration Clone()
        {
            var cfg = ScriptableObject.CreateInstance<BufferLevelConfiguration>();
            cfg.NumberOfUpgrades = NumberOfUpgrades.Clone() as IntReference;
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