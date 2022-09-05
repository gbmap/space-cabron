using System;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay.Level
{
    public class BossLevelLoader : BaseLevelLoader<BossLevelConfiguration>
    {
        public BossLevelLoader(
            BossLevelConfiguration levelConfiguration, 
            MonoBehaviour coroutineStarter, 
            string sceneName, 
            Action OnFinishedLoading
        ) : base(levelConfiguration, coroutineStarter, sceneName, OnFinishedLoading)
        {}

        protected override void ConfigureLevel()
        {
            base.ConfigureLevel();

            GameObject bossPrefab = levelConfiguration.PossibleBosses.GetNext();
            GameObject bossInstance = GameObject.Instantiate(bossPrefab,
                                                             Vector3.up * 5f,
                                                             Quaternion.identity);
            AnimationBrain.Play<BossIntroBrain>(bossInstance);
        }

        public override void Unload()
        {
            base.Unload();
        }
    }

    [CreateAssetMenu(menuName="Space Cabrón/Level/Boss")]
    public class BossLevelConfiguration : BaseLevelConfiguration
    {
        public GameObjectPool PossibleBosses;

        public override ILevelConfiguration Clone()
        {
            BossLevelConfiguration clone 
                = CreateInstance<BossLevelConfiguration>();
            clone.PossibleBosses = PossibleBosses;
            return clone;
        }

        public override ILevelLoader GetLoader(Action OnFinishedLoading = null)
        {
            return new BossLevelLoader(this, LevelLoader.CoroutineStarter, "Gameplay", OnFinishedLoading);
        }
    }
}