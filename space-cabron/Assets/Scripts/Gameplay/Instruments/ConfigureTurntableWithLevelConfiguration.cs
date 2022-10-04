using UnityEngine;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Instruments;
using SpaceCabron.Gameplay;
using System.Linq;

namespace SpaceCabron.Gameplay
{
    public class ConfigureTurntableWithLevelConfiguration : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public bool IgnoreConfigurationBPM = false;
        public bool ReloadMelody = true;
        private bool wasConfigured = false;
        public bool ResetImproviser = true;

        public bool OverrideTag;
        public string Tag;

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

            if (!wasConfigured || ReloadMelody) {
                var factory = instrumentConfig.GetMelodyFactory(false);
                MelodySwitcher ms = GetComponentInChildren<MelodySwitcher>();
                if (ms != null) {
                    ms.Generate(factory);
                }

                instrumentConfig.ConfigureTurntable(turntable, false);
                IMelodyPlayer.Generate(gameObject, factory);
            }

            if (ResetImproviser) {
                turntable.Improviser.Clear();
            }
        }

        private void LoadInstrument(InstrumentConfiguration instrumentConfig)
        {
            if (instrumentConfig.PossibleStartingInstruments == null)
                return;

            // Shouldn't know about HelmProxy.
            HelmProxy helmProxy = GetComponent<HelmProxy>();
            helmProxy.LoadPatch(instrumentConfig.PossibleStartingInstruments.GetNext());
        }

        public static void LoadInstrument(GameObject instance, InstrumentConfiguration instrumentConfig)
        {
            if (instrumentConfig.PossibleStartingInstruments == null)
                return;

            // Shouldn't know about HelmProxy.
            HelmProxy helmProxy = instance.GetComponentInChildren<HelmProxy>();
            helmProxy.LoadPatch(instrumentConfig.PossibleStartingInstruments.GetNext());

            if (instance.CompareTag("Player")) {
                helmProxy.Channel = 0;
                if (instance.name[instance.name.Length-1]-'0' == 2) {
                    helmProxy.Channel = 8;
                }
            }
        }

        private string GetTag()
        {
            string[] ignoreTags = new string[] { 
                "Untagged",
                "GlobalInstruments"
            };

            if (OverrideTag)
                return Tag;
            if (transform.parent == null || ignoreTags.Contains(transform.parent.tag))
                return gameObject.tag;
            return transform.parent.tag;
        }
    }

}