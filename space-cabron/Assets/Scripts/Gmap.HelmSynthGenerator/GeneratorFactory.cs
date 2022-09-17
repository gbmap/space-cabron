namespace Gmap.HelmSynthGenerator
{
    public class GeneratorFactory {
        public static HelmSynthGenerator Create(
            EGeneratorProfile profile,
            bool onlyExistingFields=false,
            float randomRange=0.25f
        ) {
            switch (profile) {
                case EGeneratorProfile.Lead:
                    return new HelmSynthGenerator(new LeadProfile(), onlyExistingFields, randomRange);
                case EGeneratorProfile.Ambient:
                    return new HelmSynthGenerator(new AmbientProfile(), onlyExistingFields, randomRange);
                case EGeneratorProfile.Drum:
                    return new HelmSynthGenerator(new DrumProfile(), onlyExistingFields, randomRange);
                default:
                    return new HelmSynthGenerator(new LeadProfile(), onlyExistingFields, randomRange);
            }
        }
    }
}

