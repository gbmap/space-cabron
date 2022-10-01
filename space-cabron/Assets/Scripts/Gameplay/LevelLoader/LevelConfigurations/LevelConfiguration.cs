using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay.Level;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public enum EDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    [System.Serializable]
    public class GameplayConfiguration : ICloneable<GameplayConfiguration>
    {
        public EDifficulty Difficulty;
        public int ScoreThreshold = 5000;
        public int EnemyBPMIncreaseValue = 1;
        public int EnemyBPMScoreModulusToIncrease = 25;
        public int MaxEnemiesAlive = 10;
        public GameObjectPool EnemyPool;
        public GameObjectPool BossPool;

        public GameplayConfiguration Clone()
        {
            return new GameplayConfiguration
            {
                Difficulty = Difficulty,
                ScoreThreshold = ScoreThreshold,
                EnemyPool = EnemyPool != null ? EnemyPool.Clone() as GameObjectPool : null,
                BossPool = BossPool != null ? BossPool.Clone() as GameObjectPool : null
            };
        }
    }

    [System.Serializable]
    public class ImprovisationConfiguration : ICloneable<ImprovisationConfiguration>
    {
        public IntBusReference NumberOfStartingImprovisations;
        public ImprovisationPool PossibleStartingImprovisations;
        public ImprovisationPool FixedStartingimprovisations;

        public void Apply(ITurntable turntable)
        {
            if (NumberOfStartingImprovisations != null && PossibleStartingImprovisations != null)
            {
                int numberOfImprovisations = NumberOfStartingImprovisations.Value;
                for (int i = 0; i< numberOfImprovisations; i++)
                {
                    Improvisation improvisation = PossibleStartingImprovisations.GetNext().Get(); 
                    turntable.ApplyImprovisation(improvisation, false);
                }
            }
            
            if (FixedStartingimprovisations != null)
            {
                foreach (ScriptableImprovisation improvisation in FixedStartingimprovisations.GetEnumerator())
                {
                    turntable.ApplyImprovisation(improvisation.Get(), false);
                }
            }
        }

        public ImprovisationConfiguration Clone()
        {
            return new ImprovisationConfiguration
            {
                NumberOfStartingImprovisations = NumberOfStartingImprovisations,
                PossibleStartingImprovisations = PossibleStartingImprovisations != null ? PossibleStartingImprovisations.Clone() as ImprovisationPool : null
            };
        }
    }

    [System.Serializable]
    public class BackgroundConfiguration : ICloneable<BackgroundConfiguration>
    {
        public Material Material;

        public BackgroundConfiguration Clone()
        {
            return new BackgroundConfiguration
            {
                Material = new Material(Material)
            };
        }
    }

    [CreateAssetMenu(menuName="Space Cabrón/Level/Enemy")]
    public class LevelConfiguration : BaseLevelConfiguration
    {
        public GameplayConfiguration Gameplay;
        [SerializeField] private InstrumentConfiguration EnemyMelody;
        public BackgroundConfiguration Background;

        private Dictionary<string, InstrumentConfiguration> dictTagToInstrument;
        private Dictionary<string, InstrumentConfiguration> DictTagToInstrument {
            get {
                if (dictTagToInstrument == null)
                {
                    dictTagToInstrument = new Dictionary<string, InstrumentConfiguration>();
                    dictTagToInstrument.Add("Player", Resources.Load<InstrumentConfiguration>("PlayerInstrumentConfiguration"));
                    dictTagToInstrument.Add("Metronome", Resources.Load<InstrumentConfiguration>("MetronomeInstrumentConfiguration"));
                    dictTagToInstrument.Add("Ambient", Resources.Load<InstrumentConfiguration>("AmbientInstrumentConfiguration"));
                    dictTagToInstrument.Add("Hit", Resources.Load<InstrumentConfiguration>("EnemyHitInstrumentConfiguration"));
                }
                return dictTagToInstrument;
            }
        }

        public InstrumentConfiguration GetInstrumentConfigurationByTag(string tag)
        {
            InstrumentConfiguration instrumentConfig;
            if (!DictTagToInstrument.TryGetValue(tag, out instrumentConfig))
            {
                Debug.LogWarning($"Couldn't find instrument config for {tag}.");
                Debug.Break();
                return null;
            }
            return instrumentConfig;
        }

        public override ILevelLoader GetLoader(System.Action OnFinishedLoading =null)
        {
            return new EnemyLevelLoader(this, LevelLoader.CoroutineStarter, OnFinishedLoading);
        }

        public override ILevelConfiguration Clone()
        {
            LevelConfiguration clone = ScriptableObject.CreateInstance<LevelConfiguration>();
            clone.Background = Background.Clone();
            clone.Gameplay = Gameplay.Clone();
            clone.EnemyMelody = EnemyMelody?.Clone();
            return clone;
        }
    }

}