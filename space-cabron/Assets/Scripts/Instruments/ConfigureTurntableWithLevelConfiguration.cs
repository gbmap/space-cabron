using UnityEngine;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using SpaceCabron.Instruments;

namespace SpaceCabron.Gameplay
{
    public class ConfigureTurntableWithLevelConfiguration : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public void Configure(LevelConfiguration configuration)
        {
            var injectMelody = GetComponent<InjectTurntableMelodyNotationOnAwake>();
            if (injectMelody)
                DestroyImmediate(injectMelody);

            ITurntable turntable = GetComponent<ITurntable>();
            MelodyConfiguration melodyConfig = configuration.GetMelodyConfigurationByTag(gameObject.tag);
            turntable.BPM = melodyConfig.BPM;
            turntable.SetMelody(melodyConfig.StartingMelody);
        }
    }
}