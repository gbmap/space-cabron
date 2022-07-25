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
        public int BPM;
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
        public MelodyConfiguration Melody;
        public BackgroundConfiguration Background;
    }

    public interface ILevelConfigurable
    {
        void Configure(LevelConfiguration configuration);
    }
}