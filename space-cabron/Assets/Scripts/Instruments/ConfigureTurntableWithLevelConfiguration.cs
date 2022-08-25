using UnityEngine;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Instruments;
using SpaceCabron.Gameplay;

namespace SpaceCabron.Gameplay
{
    public class ConfigureTurntableWithLevelConfiguration : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public bool IgnoreConfigurationBPM = false;
        public bool ReloadMelody = true;
        private bool wasConfigured = false;
        public void Configure(LevelConfiguration configuration)
        {
            var injectables = GetComponentsInChildren<Injectable>();
            System.Array.ForEach(injectables, i => Destroy(i));

            InstrumentConfiguration instrumentConfig = configuration.GetInstrumentConfigurationByTag(GetTag());
            LoadInstrument(instrumentConfig);
            SetupBPMAndMelody(instrumentConfig);
            wasConfigured=true;
        }

        private void SetupBPMAndMelody(InstrumentConfiguration instrumentConfig)
        {
            ITurntable turntable = GetComponentInChildren<ITurntable>();
            if (turntable == null)
                return;

            if (!wasConfigured || !IgnoreConfigurationBPM)
            {
                turntable.BPM = instrumentConfig.BPM;
                turntable.MaxBPM = instrumentConfig.MaxBPM;
            }

            if (!wasConfigured || ReloadMelody)
                instrumentConfig.ConfigureTurntable(turntable, false);
        }

        private void LoadInstrument(InstrumentConfiguration instrumentConfig)
        {
            if (instrumentConfig.PossibleStartingInstruments == null)
                return;

            // Shouldn't know about HelmProxy.
            HelmProxy helmProxy = GetComponent<HelmProxy>();
            helmProxy.LoadPatch(instrumentConfig.PossibleStartingInstruments.GetNext());
        }

        private string GetTag()
        {
            if (transform.parent == null || transform.parent.tag == "Untagged")
                return gameObject.tag;
            return transform.parent.tag;
        }
    }

}