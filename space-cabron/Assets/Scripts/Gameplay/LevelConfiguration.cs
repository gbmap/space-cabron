using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public enum EDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class BackgroundConfiguration
    {
        public Material Material;
    }

    public class LevelConfiguration
    {
        public EDifficulty Difficulty;

        public int BPM;
        public Melody Melody;
        public BackgroundConfiguration Background;
    }

    public interface ILevelConfigurable
    {
        void Configure(LevelConfiguration configuration);
    }
}