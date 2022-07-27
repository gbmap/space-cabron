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
            var injectables = GetComponentsInChildren<Injectable>();
            System.Array.ForEach(injectables, i => DestroyImmediate(i));

            InstrumentConfiguration instrumentConfig = configuration.GetMelodyConfigurationByTag(gameObject.tag);
            ITurntable turntable = GetComponent<ITurntable>();
            if (turntable != null)
            {
                turntable.BPM = instrumentConfig.BPM;
                turntable.SetMelody(instrumentConfig.StartingMelody);
            }

            if (instrumentConfig.PossibleStartingInstruments != null)
            {
                // Shouldn't know about HelmProxy.
                HelmProxy helmProxy = GetComponent<HelmProxy>();
                helmProxy.LoadPatch(instrumentConfig.PossibleStartingInstruments.GetNext());
            }
        }
    }
}