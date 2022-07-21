

using System.Collections.Generic;

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

        private static Dictionary<string, ENote> dictCharToNote = new Dictionary<string, ENote>
        {
            { "a", ENote.A },
            { "a#", ENote.Asharp},
            { "b", ENote.B },
            { "c", ENote.C },
            { "c#", ENote.Csharp },
            { "d", ENote.D },
            { "d#", ENote.Dsharp },
            { "e", ENote.E },
            { "f", ENote.F },
            { "f#", ENote.Fsharp },
            { "g", ENote.G },
            { "g#", ENote.Gsharp },
        };
        
        public static ENote FromString(string s) {
            return dictCharToNote[s];
        }
    }
}