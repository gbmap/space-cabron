using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class ConfigureTurntableWithLevelConfiguration : MonoBehaviour, ILevelConfigurable
    {
        public void Configure(LevelConfiguration configuration)
        {
            ITurntable turntable = GetComponent<ITurntable>();
            turntable.SetMelody(configuration.Melody.StartingMelody);
        }
    }
}