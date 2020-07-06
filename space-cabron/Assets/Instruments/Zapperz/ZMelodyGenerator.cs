using QFSW.QC;
using System;
using UnityEngine;

namespace Z
{
    /*
     * 
     * MELODY GENERATOR
     * Algorithms to generate melodies. Basically a ZMelody factory.
     * 
     * */
    public enum EMelodyAlgorithm
    {
        SkipAndWalk,
        RunThrough
    }

    [System.Serializable]
    public class ZMelody
    {
        public static ZMelody Empty
        {
            get => new ZMelody { notes = new ENote[0], octaves = new int[0] };
        }

        public ENote root;
        public EScale scale;

        public ENote[] notes;
        public int[] octaves;

        public int Length
        {
            get { return notes.Length; }
        }

        private int Cursor;
        public int CurrentNoteIndex { get { return Cursor; } }
        public ENote CurrentNote
        {
            get { return notes[Cursor]; }
        }

        public int CurrentOctave
        {
            get { return octaves[Cursor]; }
        }

        public bool IsEmpty { get => notes.Length == 0; }

        public ZMelody() { }

        public ZMelody(ZMelody m)
        {
            root = m.root;
            scale = m.scale;
            notes = new ENote[m.notes.Length];
            Array.Copy(m.notes, notes, m.notes.Length);
            octaves = new int[m.octaves.Length];
            Array.Copy(m.octaves, octaves, m.octaves.Length);
            Cursor = 0;
        }

        public void Advance()
        {
            Cursor = (Cursor + 1) % notes.Length;
        }

        public override string ToString()
        {
            string s = string.Empty;
            s += "Root: " + root + "\n";
            s += "Scale: " + scale + "\n";

            s += "Notes: [";
            foreach (var n in notes)
            {
                s += n.ToString() + ", ";
            }
            s += "]\n";

            s += "Octaves: [";
            foreach (var o in octaves)
            {
                s += o + ", ";
            }
            s += "]\n";

            s += "Current Note: " + CurrentNoteIndex + "\n";
            return s;
        }
    }

    [CommandPrefix("melodyGen.")]
    public class ZMelodyGenerator
    {
        /*
         * Just runs through a scale without repeating the root an octave above.
         * */
        [Command("runThrough")]
        public static ZMelody GenerateRunThroughMelody(ENote root, EScale scale, int sOctave, int nNotes)
        {
            ENote[] notes = new ENote[nNotes];
            int[] octaves = new int[nNotes];

            ENote[] scaleNotes = Zapperz.GetScale(scale, root);

            for (int i = 0; i < nNotes; i++)
            {
                notes[i] = scaleNotes[i % (scaleNotes.Length - 1)]; // -1 allows to don't repeat the root note
                octaves[i] = sOctave;
            }

            return new ZMelody()
            {
                notes = notes,
                octaves = octaves,
                root = root,
                scale = scale
            };
        }

        /*
         * Melody generator similar to old dungeon generators,
         * It departs from the root note, walks towards the left or the right,
         * or skips a note left or right inside a scale, and ends on the root.
         * 
         * */
         [Command("skipAndWalk")]
        public static ZMelody GenerateSkipAndWalkMelody(ENote root, EScale scale, int sOctave, int nNotes)
        {
            ENote[] scaleNotes = Zapperz.GetScale(scale, root);
            return GenerateSkipAndWalkMelody(root, scaleNotes, sOctave, nNotes, scale);
        }

        [Command("skipAndWalk")]
        public static ZMelody GenerateSkipAndWalkMelody(ENote root, ENote[] scaleNotes, int sOctave, int nNotes, EScale scale = EScale.Custom)
        {
            ENote[] notes = new ENote[nNotes];
            int[] octaves = new int[nNotes];

            ENote n = root; // current note
            int o = sOctave; // current octave
            int position = 0; // position of the walker
            int i = 0; // which note is being generated
            while (i < nNotes)
            {
                notes[i] = n;
                octaves[i] = o;

                int direction = (int)Mathf.Sign(UnityEngine.Random.value - 0.5f);
                int step = Mathf.CeilToInt(UnityEngine.Random.value * 2f) * direction;

                if (position + step >= scaleNotes.Length)
                {
                    o++;
                }

                position = (position + step) % scaleNotes.Length;
                if (position < 0) // position is negative, wrap around octave
                {
                    o--;
                    position = scaleNotes.Length + position;
                }

                n = scaleNotes[position];
                i++;
            }

            return new ZMelody
            {
                root = root,
                scale = scale,
                notes = notes,
                octaves = octaves
            };
        }
    }
}