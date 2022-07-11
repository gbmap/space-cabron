

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

    [System.Serializable]
    public class Note
    {
        public ENote Tone;
        public int Interval;

        public static ENote OffsetNote(ENote n, int steps)
        {
            return (ENote)(Bar.MathMod(((int)n + steps) , 12));
        }

        public static int ToMIDI(ENote n, int octave)
        {
            return (12 * octave) + (int)n;
        }

        public float GetTime(float bps)
        {
            return (4f/Interval)*bps;
        }
    }
}