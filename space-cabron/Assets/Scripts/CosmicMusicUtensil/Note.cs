

namespace Gmap.CosmicMusicUtensil
{
    public enum ENote
    {
        C,
        Csharp,
        D,
        Dsharp,
        E,
        F,
        Fsharp,
        G,
        Gsharp,
        A,
        Asharp,
        B,
        None
    }

    public class Note
    {
        public static ENote OffsetNote(ENote n, int steps)
        {
            return (ENote)(((int)n + steps) % 12);
        }

        public static int ToMIDI(ENote n, int octave)
        {
            return (12 * octave) + (int)n;
        }
    }
}