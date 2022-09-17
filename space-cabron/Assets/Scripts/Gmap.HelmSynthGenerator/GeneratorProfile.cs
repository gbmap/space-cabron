namespace Gmap.HelmSynthGenerator
{
    public enum EGeneratorProfile {
        Lead,
        Ambient,
        Drum
    }

    public class GeneratorProfile {
        public BaseHelmParameters[] ParametersArray { get; private set; }
        public BaseHelmParameters Parameters { get => ParametersArray[0]; }  
        public GeneratorProfile(params BaseHelmParameters[] parameters) {
            this.ParametersArray= parameters;
            var baseParams = ParametersArray[0];
            for (int i = 1; i < ParametersArray.Length; i++) {
                baseParams = baseParams.Append(ParametersArray[i]);
            }
        }
    }

    public class LeadProfile : GeneratorProfile {
        public LeadProfile() : base(
            new AmplitudeEnvelopeParameters(
                attackMin: 0.01f,
                attackMax: 0.1f,
                decayMin: 0.1f,
                decayMax: 0.5f,
                sustainMin: 0.8f,
                sustainMax: 1.0f,
                releaseMin: 0.3f,
                releaseMax: 1.0f
            ),
            new OscHelmParameters(1),
            new OscHelmParameters(2),
            new SubOscHelmParameters(),
            new LFOParameters(1)
        ) {}
    }

    public class AmbientProfile : GeneratorProfile {
        public AmbientProfile() : base(
            new AmplitudeEnvelopeParameters(
                attackMin: 0.0f,
                attackMax: 3f,
                decayMin: 0.0f,
                decayMax: 3f,
                sustainMin: 0f,
                sustainMax: 1f,
                releaseMin: 2f,
                releaseMax: 3f
            ),
            new OscHelmParameters(1),
            new OscHelmParameters(2),
            new SubOscHelmParameters(),
            new ArpParameters(),
            new FilterParameters(),
            new FormantParameters(),
            new LFOParameters(1),
            new LFOParameters(2)
        ) {}
    }

    public class DrumProfile : GeneratorProfile {
        public DrumProfile() : base(
            new AmplitudeEnvelopeParameters(
                attackMin: 0.0f,
                attackMax: 0.1f,
                releaseMin: 0.1f,
                releaseMax: 0.2f
            ),
            new OscHelmParameters(1),
            new OscHelmParameters(2),
            new SubOscHelmParameters()
        ) {}
    }
}
