using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public Note(Note n)
        {
            Tone = n.Tone;
            Interval = n.Interval;
            Octave = n.Octave;
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

        public float GetDuration(float bps)
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
            { "-", ENote.None }
        };

        
        public static ENote FromString(string s) {
            return dictCharToNote[s];
        }

        public static string ToString(ENote n) {
            return dictCharToNote.FirstOrDefault(x => x.Value == n).Key;
        }

        public string AsString() {
            return $"{Note.ToString(Tone)}{Octave}/{Interval}";
        }

        public void Transpose(int interval) {
            if (interval == 0)
                return; 
            else if (interval > 0)
                Octave += System.Convert.ToInt32(Mathf.Abs(12-(int)Tone)<=interval)*Mathf.Max(1, interval/12);
            else if (interval < 0)
                Octave += System.Convert.ToInt32(((int)Tone)+interval<0)*Mathf.Min(-1, interval/12);
            Tone = OffsetNote(Tone, interval);
        }

        public static Note Transpose(Note n, int interval)
        {
            Note n2 = new Note(n);
            n2.Transpose(interval);
            return n2;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
                return false;

            Note n2 = (Note)obj;
            return (Tone == n2.Tone) && (Interval == n2.Interval) && (Octave == n2.Octave);
        }

        public static bool operator==(Note n1, Note n2) {
            if (System.Object.ReferenceEquals(n1, n2))
                return true;

            if (((object)n1 == null) || ((object)n2 == null))
                return false;

            return n1.Equals(n2);
        }

        public static bool operator!=(Note n1, Note n2) {
            if (System.Object.ReferenceEquals(n1, n2))
                return false;

            if (((object)n1 == null) || ((object)n2 == null))
                return false;

            return !n1.Equals(n2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}