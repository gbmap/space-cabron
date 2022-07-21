

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
        public int Octave;

        public Note()
        {
            Tone = ENote.None;
            Interval = 0;
            Octave = 0;
        }

        public Note(ENote tone, int interval, int octave)
        {
            Tone = tone;
            Interval = interval;
            Octave = octave;
        }

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