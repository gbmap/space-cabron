

namespace Gmap.CosmicMusicUtensil
{
    public interface IIntervalsGenerator
    {
        int[] Generate(IntervalsGeneratorParams p);
    }

    public class IntervalsGeneratorParams
    {
        public int MinfNotes;
        public int MaxNotes;
    }

    public class EvenlySpacedIntervalsGenerator : IIntervalsGenerator
    {
        public EvenlySpacedIntervalsGenerator()
        {

        }

        public int[] Generate(IntervalsGeneratorParams p)
        {
            return null;

        }
    }
}