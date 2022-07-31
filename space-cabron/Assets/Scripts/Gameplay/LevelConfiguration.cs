using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.Gameplay
{
    public enum EDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    [System.Serializable]
    public class GameplayConfiguration
    {
        public EDifficulty Difficulty;
        public int ScoreThreshold = 5000;
        public GameObjectPool EnemyPool;
        public GameObjectPool BossPool;
    }

    [System.Serializable]
    public class InstrumentConfiguration
    {
        public int BPM = 30;
        public ScriptableMelodyFactory MelodyFactory;
        public TextAssetPool PossibleStartingInstruments;
    }

    [System.Serializable]
    public class BackgroundConfiguration
    {
        public Material Material;
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
                    dictTagToInstrument.Add("Enemy", EnemyMelody);
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
            clone.Background = Background;
            clone.Gameplay = Gameplay;
            clone.EnemyMelody = EnemyMelody;
            clone.DrumMelody = DrumMelody;
            clone.PlayerMelody = PlayerMelody;
            clone.AmbientMelody = AmbientMelody;
            clone.HitConfiguration = HitConfiguration;
            clone.NextLevel = NextLevel;
            return clone;
        }
    }

}