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
    public class GameplayConfiguration
    {
        public EDifficulty Difficulty;
        public int ScoreThreshold = 5000;
        public int StartingBPM = 30;
        public GameObjectPool EnemyPool;
        public GameObjectPool BossPool;
    }

    [System.Serializable]
    public class MelodyConfiguration
    {
        public int BPM = 30;
        public Melody StartingMelody;
        public StringReferencePool PossibleStartingMelodies;
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
        [SerializeField] private MelodyConfiguration EnemyMelody;
        [SerializeField] private MelodyConfiguration PlayerMelody;
        [SerializeField] private MelodyConfiguration DrumMelody;
        public BackgroundConfiguration Background;

        public MelodyConfiguration GetMelodyConfigurationByTag(string tag)
        {
            if (tag == "Player")
                return PlayerMelody;
            else if (tag == "Enemy")
            {
                EnemyMelody.StartingMelody = new Melody(EnemyMelody.PossibleStartingMelodies.GetNext().Value);
                return EnemyMelody;
            }
            else
            {
                DrumMelody.StartingMelody = new Melody(DrumMelody.PossibleStartingMelodies.GetNext().Value);
                return DrumMelody;
            }
        }

        public LevelConfiguration Clone()
        {
            LevelConfiguration clone = ScriptableObject.CreateInstance<LevelConfiguration>();
            clone.Background = Background;
            clone.Gameplay = Gameplay;
            clone.EnemyMelody = EnemyMelody;
            clone.DrumMelody = DrumMelody;
            clone.PlayerMelody = PlayerMelody;
            return clone;
        }
    }

}