

namespace Gmap.CosmicMusicUtensil
{
    public interface IIntervalsGenerator
    {
        int[] Generate();
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

        public int[] Generate()
        {
            return null;

        }
    }
}