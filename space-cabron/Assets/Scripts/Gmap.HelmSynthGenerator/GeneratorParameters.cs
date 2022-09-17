using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.HelmSynthGenerator {
    public abstract class BaseHelmParameters {
        protected abstract Dictionary<string, GenericParameter> GetParameters();
        protected IEnumerable<HelmModulation> GetModulations() {
            var sources = GetSources();
            var destinations = GetDestinations();
            foreach (var source in sources) {
                foreach (var destination in destinations) {
                    yield return new HelmModulation(source, destination, new ModulationParam());
                }
            }
        }
        protected virtual List<string> GetDestinations() {
            return new List<string>();
        }

        protected virtual List<string> GetSources() {
            return new List<string>() {
                "amp_envelope",
                "amp_envelope_amp",
                "amp_envelope_phase",
                // "note",
                // "peak_meter",
                "velocity"
            };
        }

        public Dictionary<string, GenericParameter> Parameters {
            get {
                var parameters = new Dictionary<string, GenericParameter>();
                if (parent != null) {
                    parameters = parent.Parameters;
                }
                Merge(parameters, GetParameters());
                return parameters;
            }
        }

        public List<HelmModulation> Modulations {
            get {
                var modulations = new List<HelmModulation>();
                if (parent != null) {
                    modulations = parent.Modulations;
                }
                modulations.AddRange(GetModulations());
                return modulations;
            }
        }

        BaseHelmParameters parent = null;
        public BaseHelmParameters(BaseHelmParameters parent=null) {
            if (parent != null)
                Append(parent);
        }

        public BaseHelmParameters Append(BaseHelmParameters parameters) {
            this.parent = parameters;
            return this.parent;
        }

        public bool HasValue(string name) {
            return Parameters.ContainsKey(name);
        }

        public float GetValue(string name) {
            if (Parameters.ContainsKey(name)) {
                return Parameters[name].GetValue();
            }
            return 0f;
        }

        public Vector2 GetRange(string name) {
            if (Parameters.ContainsKey(name)) {
                return Parameters[name].GetRange();
            }
            return new Vector2(0f, 0f);
        }

        public void Merge<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (second == null || first == null) return;
            foreach (var item in second) 
                if (!first.ContainsKey(item.Key)) 
                    first.Add(item.Key, item.Value);
        }
    }

    public abstract class EnvelopedParameters : BaseHelmParameters {
        public FloatParameter Attack;
        public FloatParameter Decay;
        public FloatParameter Sustain;
        public FloatParameter Release;

        public EnvelopedParameters(
            float attackMin=0.01f,
            float attackMax=1.0f,
            float decayMin=0.5f,
            float decayMax=0.5f,
            float sustainMin=0.0f,
            float sustainMax=1.0f,
            float releaseMin=0.08f,
            float releaseMax=2f,
            BaseHelmParameters parent = null
        ) : base(parent) {
            Attack = new FloatParameter(attackMin, attackMax);
            Decay = new FloatParameter(decayMin, decayMax);
            Sustain = new FloatParameter(sustainMin, sustainMax);
            Release = new FloatParameter(releaseMin, releaseMax);
        }
    }

    public class AmplitudeEnvelopeParameters : EnvelopedParameters
    {
        public AmplitudeEnvelopeParameters(
            float attackMin=0.01f,
            float attackMax=1.0f,
            float decayMin=0.5f,
            float decayMax=0.5f,
            float sustainMin=0.0f,
            float sustainMax=1.0f,
            float releaseMin=0.08f,
            float releaseMax=2f,
            BaseHelmParameters parent = null
        ) : base(attackMin, attackMax, decayMin, decayMax, sustainMin, sustainMax, releaseMin, releaseMax, parent) {}

        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter>() {
                { "amp_attack", Attack },
                { "amp_decay", Decay },
                { "amp_sustain", Sustain },
                { "amp_release", Release },
                { "num_modulations", new IntParameter(5,7) },
                { "polyphony", new IntParameter(1, 32) }
            };
        }
    }

    public class OscHelmParameters : BaseHelmParameters
    {
        int oscillatorIndex;
        public OscHelmParameters(int oscillatorIndex) {
            this.oscillatorIndex = oscillatorIndex;
        }
        protected override List<string> GetDestinations()
        {
            return new List<string> {
                $"osc_{oscillatorIndex}_volume",
                $"osc_{oscillatorIndex}_transpose",
                $"osc_{oscillatorIndex}_tune"
            };
        }

        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter>() {
                { $"osc_{oscillatorIndex}_transpose", new FloatParameter(-48f, 48f) },
                { $"osc_{oscillatorIndex}_tune", new FloatParameter(-100f, 100f) },
                { $"osc_{oscillatorIndex}_unison_detune", new FloatParameter(0f, 100f) },
                { $"osc_{oscillatorIndex}_unison_voices", new IntParameter(1, 15) },
                { $"osc_{oscillatorIndex}_volume", new OscillatorVolumeParam(1f, 1f) }, 
                { $"osc_{oscillatorIndex}_waveform", new WaveformParameter() }
            };
        }
    }

    public class SubOscHelmParameters : BaseHelmParameters
    {
        protected override List<string> GetDestinations()
        {
            return new List<string> {
                "sub_octave",
                "sub_shuffle",
                "sub_volume",
                "sub_waveform"
            };
        }

        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter> {
                { "sub_octave", new IntParameter(0, 2) },
                { "sub_shuffle", new FloatParameter(0f, 1f) },
                { "sub_volume", new OscillatorVolumeParam(0.125f, 0.125f) },
                { "sub_waveform", new WaveformParameter() },
            }; 
        }
    }

    public class ArpParameters : BaseHelmParameters {
        protected override List<string> GetDestinations()
        {
            return new List<string>() {
                "arp_frequency",
                "arp_gate",
                "arp_octaves",
                "arp_pattern",
                "arp_tempo"
            };
        }

        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter>() {
                { "arp_on", new ProbBoolParameter(0.125f) },
                { "arp_frequency", new IntParameter(0, 4) },
                { "arp_gate", new FloatParameter(0f, 1.0f) },
                { "arp_octaves", new IntParameter(1, 4) },
                { "arp_pattern", new IntParameter(1, 5) },
                { "arp_sync", new IntParameter(1, 1) },
                { "arp_tempo", new IntParameter(6, 10) },
            };
        }
    }

    public class FilterParameters : BaseHelmParameters {
        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter> {
                { "filter_on", new ProbBoolParameter(0.125f) },
                { "filter_blend", new FloatParameter(0f, 2f) },
                { "filter_drive", new FloatParameter(4f, 4f) },
                { "filter_shelf", new IntParameter(0, 3) },
                { "filter_style", new IntParameter(0, 3) },
                { "fil_env_depth", new FloatParameter(-128f, 128f) },
                { "filter_saturation", new FloatParameter(60f, 127f) },
                { "fil_attack", new EnvParameterA() },
                { "fil_decay", new EnvParameterD() }, 
                { "fil_release", new EnvParameterR() },
                { "fil_sustain", new EnvParameterS() }
            };
        }

        protected override List<string> GetDestinations()
        {
            return new List<string>() {
                "filter_blend",
                "filter_drive",
                "fil_env_depth",
                "fil_envelope",
                "fil_envelope_amp",
                "fil_envelope_phase",
                "fil_attack",
                "fil_decay",
                "fil_release",
                "fil_sustain"
            };
        }
    }

    public class FormantParameters : BaseHelmParameters
    {
        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter>() {
                { "formant_on", new ProbBoolParameter(0.125f) },
                { "formant_x", new FloatParameter(0f, 1f) },
                { "formant_y", new FloatParameter(0f, 1f) }
            };
        }

        protected override List<string> GetDestinations()
        {
            return new List<string>() {
                "formant_x",
                "formant_y"
            };
        }
    }

    public class LFOParameters : BaseHelmParameters
    {
        int LFOIndex;
        public LFOParameters(int lfoIndex) {
            LFOIndex = lfoIndex;
        }
        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return new Dictionary<string, GenericParameter>() {
                { $"mono_lfo_{LFOIndex}_amplitude", new FloatParameter(0f, 1f) },
                { $"mono_lfo_{LFOIndex}_frequency", new IntParameter(0, 5) },
                { $"mono_lfo_{LFOIndex}_retrigger", new IntParameter(0, 1) },
                { $"mono_lfo_{LFOIndex}_sync", new IntParameter(0, 2) },
                { $"mono_lfo_{LFOIndex}_tempo", new IntParameter(0,8) },
                { $"mono_lfo_{LFOIndex}_waveform", new WaveformParameter() },
            };
        }

        protected override List<string> GetSources()
        {
            return new List<string>(){ 
                $"mono_lfo_{LFOIndex}",
                $"mono_lfo_{LFOIndex}_phase" 
            };
        }

        protected override List<string> GetDestinations()
        {
            return new List<string>() {
                $"mono_lfo_{LFOIndex}_amplitude",
                $"mono_lfo_{LFOIndex}_frequency",
                $"mono_lfo_{LFOIndex}_tempo",
                $"mono_lfo_{LFOIndex}_waveform"
            };
        }
    }

    public class HelmParametersGenerator : BaseHelmParameters {
        public Dictionary<string, GenericParameter> ParametersRange = new Dictionary<string, GenericParameter>() {
            { "amp_attack", new EnvParameterA() },
            { "amp_decay", new EnvParameterD() },
            { "amp_sustain", new EnvParameterS() },
            { "amp_release", new EnvParameterR() },
            { "arp_frequency", new IntParameter(0, 4) },
            { "arp_gate", new FloatParameter(0f, 1.0f) },
            { "arp_octaves", new IntParameter(1, 4) },
            { "arp_on", new ProbBoolParameter(0.25f) },
            { "arp_pattern", new IntParameter(1, 5) },
            { "arp_sync", new IntParameter(1, 1) },
            { "arp_tempo", new IntParameter(6, 10) },
            { "beats_per_minute", new IntParameter(60, 120) },
            { "cross_modulation", new FloatParameter(0f, 1f) },
            // { "cutoff", new FloatParameter(0f, 100f) },
            // { "delay_dry_wet", new FloatParameter(0f, 1f) },
            // { "delay_feedback", new FloatParameter(0f, 1f) },
            // { "delay_frequency", new IntParameter(0, 2) },
            // { "delay_on", new IntParameter(0, 1) },
            // { "delay_sync", new IntParameter(0, 1) },
            // { "delay_tempo", new IntParameter(0, 10) },
            // { "distortion_drive", new IntParameter(0, 12) },
            // { "distortion_mix", new FloatParameter(0f, 1f)} ,
            // { "distortion_on", new IntParameter(0, 1) },
            // { "distortion_type", new IntParameter(0, 4) }, 
            { "filter_on", new ProbBoolParameter(0.25f) },
            { "fil_attack", new EnvParameterA() },
            { "fil_decay", new EnvParameterD() }, 
            { "fil_release", new EnvParameterR() },
            { "fil_sustain", new EnvParameterS() },
            { "fil_env_depth", new FloatParameter(-128f, 128f) },
            { "filter_blend", new FloatParameter(0f, 2f) },
            { "filter_drive", new FloatParameter(4f, 4f) },
            { "filter_shelf", new IntParameter(0, 3) },
            { "filter_style", new IntParameter(0, 3) },
            { "formant_on", new ProbBoolParameter(0.1f) },
            { "formant_x", new FloatParameter(0f, 1f) },
            { "formant_y", new FloatParameter(0f, 1f) },
            { "keytrack", new IntParameter(0, 1) },
            { "legato", new IntParameter(0, 1) },
            { "mod_attack", new FloatParameter(1f, 1f) },
            { "mod_decay", new FloatParameter(0f, 3.0f) },
            { "mod_release", new FloatParameter(1f, 3.0f) },
            { "mod_sustain", new FloatParameter(0f, 1.0f) },
            { "mono_lfo_1_amplitude", new FloatParameter(0f, 1f) },
            { "mono_lfo_1_frequency", new IntParameter(0, 5) },
            { "mono_lfo_1_retrigger", new IntParameter(0, 1) },
            { "mono_lfo_1_sync", new IntParameter(0, 2) },
            { "mono_lfo_1_tempo", new IntParameter(0,8) },
            { "mono_lfo_1_waveform", new WaveformParameter() },
            { "mono_lfo_2_amplitude", new FloatParameter(0f, 1f) },
            { "mono_lfo_2_frequency", new FloatParameter(0f, 5f) },
            { "mono_lfo_2_retrigger", new IntParameter(0, 1) },
            { "mono_lfo_2_sync", new IntParameter(0, 2) },
            { "mono_lfo_2_tempo", new IntParameter(0, 8) },
            { "mono_lfo_2_waveform", new WaveformParameter() },
            { "noise_volume", new FloatParameter(0f, 0.1f) },
            // { "num_steps", new OscillatorVolumeParam(0, 9) },
            { "osc_1_transpose", new FloatParameter(-48f, 48f) },
            { "osc_1_tune", new FloatParameter(-100f, 100f) },
            { "osc_1_unison_detune", new FloatParameter(0f, 100f) },
            { "osc_1_unison_voices", new IntParameter(1, 15) },
            { "osc_1_volume", new OscillatorVolumeParam(1f) }, 
            { "osc_1_waveform", new WaveformParameter() },
            { "osc_2_transpose", new FloatParameter(-48f, 48f) },
            { "osc_2_tune", new FloatParameter(-100f, 100f) },
            { "osc_2_unison_detune", new FloatParameter(0f, 100f) },
            { "osc_2_unison_voices", new IntParameter(1, 15) },
            { "osc_2_volume", new OscillatorVolumeParam(0.5f) }, 
            { "osc_2_waveform", new WaveformParameter() },
            { "osc_feedback_amount", new FloatParameter(-1f, 1f) },
            { "osc_feedback_transpose", new IntParameter(-24, 24) },
            { "osc_feedback_tune", new FloatParameter(-1f, 1f) },
            { "pitch_bend_range", new IntParameter(0, 25) },
            { "poly_lfo_amplitude", new FloatParameter(-1f, 1f) },
            { "poly_lfo_frequency", new IntParameter(0, 6) },
            { "poly_lfo_sync", new IntParameter(0, 2) },
            { "poly_lfo_tempo", new IntParameter(0, 6) },
            { "poly_lfo_waveform", new WaveformParameter() },
            { "polyphony", new IntParameter(1, 32) },
            { "resonance", new FloatParameter(0f, 1f) } ,
            // { "reverb_damping", new FloatParameter(0f, 1f) },
            // { "reverb_dry_wet", new FloatParameter(0f, 1f) },
            // { "reverb_feedback", new FloatParameter(0f, 1f) },
            // { "reverb_on", new IntParameter(0, 2) },
            // { "step_frequency", new IntParameter(0, 6) },
            // { "stutter_frequency", new IntParameter(1, 5)},
            // { "stutter_on", new IntParameter(0, 1) },
            // { "stutter_resample_frequency", new IntParameter(1, 6) },
            // { "stutter_resample_sync", new IntParameter(0, 2) },
            // { "stutter_resample_tempo", new FloatParameter(1, 6) },
            // { "stutter_softness", new FloatParameter(0f, 1f) },
            // { "stutter_sync", new IntParameter(0, 2) },
            // { "stutter_tempo", new FloatParameter(1, 6) },
            { "sub_octave", new IntParameter(0, 2) },
            { "sub_shuffle", new FloatParameter(0f, 100f) },
            { "sub_volume", new OscillatorVolumeParam(0.125f) },
            { "sub_waveform", new WaveformParameter() },
            // { "unison_1_harmonize", new IntParameter(0, 2)},
            // { "unison_2_harmonize", new IntParameter(0, 2)},
            // { "velocity_track", new FloatParameter(-1f, 1f) },
            // { "volume", new FloatParameter(1f, 1f) }
        };

        protected override Dictionary<string, GenericParameter> GetParameters()
        {
            return ParametersRange;
        }
    }
}
