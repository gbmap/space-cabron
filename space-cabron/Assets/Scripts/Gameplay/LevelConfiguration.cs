using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
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
    public class InstrumentConfiguration : ICloneable<InstrumentConfiguration>
    {
        public int BPM = 30;
        public ScriptableMelodyFactory MelodyFactory;
        public TextAssetPool PossibleStartingInstruments;

        public InstrumentConfiguration Clone()
        {
            return new InstrumentConfiguration
            {
                BPM = BPM,
                MelodyFactory = MelodyFactory != null ? MelodyFactory.Clone() : null,
                PossibleStartingInstruments = PossibleStartingInstruments != null ? PossibleStartingInstruments.Clone() as TextAssetPool : null
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

    [CreateAssetMenu(menuName="Space Cabrón/Gameplay/Level Configuration")]
    public class LevelConfiguration : ScriptableObject
    {
        public GameplayConfiguration Gameplay;
        [SerializeField] private InstrumentConfiguration EnemyMelody;
        [SerializeField] private InstrumentConfiguration PlayerMelody;
        [SerializeField] private InstrumentConfiguration DrumMelody;
        [SerializeField] private InstrumentConfiguration AmbientMelody;
        [SerializeField] private InstrumentConfiguration HitConfiguration;
        public BackgroundConfiguration Background;

        public LevelConfiguration NextLevel;

        private Dictionary<string, InstrumentConfiguration> dictTagToInstrument;
        private Dictionary<string, InstrumentConfiguration> DictTagToInstrument {
            get {
                if (dictTagToInstrument == null)
                {
                    dictTagToInstrument = new Dictionary<string, InstrumentConfiguration>();
                    dictTagToInstrument.Add("EnemySpawner", EnemyMelody);
                    dictTagToInstrument.Add("Player", PlayerMelody);
                    dictTagToInstrument.Add("Metronome", DrumMelody);
                    dictTagToInstrument.Add("Ambient", AmbientMelody);
                    dictTagToInstrument.Add("Hit", HitConfiguration);
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

        public LevelConfiguration Clone()
        {
            LevelConfiguration clone = ScriptableObject.CreateInstance<LevelConfiguration>();
            clone.Background = Background.Clone();
            clone.Gameplay = Gameplay.Clone();
            clone.EnemyMelody = EnemyMelody.Clone();
            clone.DrumMelody = DrumMelody.Clone();
            clone.PlayerMelody = PlayerMelody.Clone();
            clone.AmbientMelody = AmbientMelody.Clone();
            clone.HitConfiguration = HitConfiguration.Clone();
            clone.NextLevel = NextLevel;
            return clone;
        }
    }

}