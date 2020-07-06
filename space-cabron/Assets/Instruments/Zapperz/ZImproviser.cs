using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Z
{
    #region MELODY IMPROVISER

    ////////////////////////////////////////////////////////////
    /// MELODY IMPROVISER 
    ///////////////////////////////////////////////////////////

    /*
     * Generates note values for a bar of music
     * */
    public class ZMelodyImproviser
    {
        public bool HasImprovisers => Improvisers.Count > 0;

        public List<ZNoteImproviser> Improvisers = new List<ZNoteImproviser>();

        public ZMelody Improvise(ZMelody melody, int barIndex)
        {
            ZMelody m = new ZMelody(melody);
            foreach (var imp in Improvisers)
            {
                m = imp.Improvise(m, barIndex);
            }

            return m;
        }

        public void Add(ZNoteImproviser imp)
        {
            Improvisers.Add(imp);
        }
    }

    public abstract class ZNoteImproviser
    {
        public abstract ZMelody Improvise(ZMelody melody, int currentBar);
    }

    public class ZMelodyImproviserAltNotes : ZNoteImproviser
    {
        ///////////////
        /// here's where's free will show us his middle finger and we are to
        /// decided whether or not we are to change notes in a per bar equivalent
        /// or a per instanciating of this class.
        /// these are two different solutions and will result in two different major effects
        /// one simply alternates between two sets of different notes per numberOfBarChanges and
        /// the other plays a completely different melody per numberOfBarChanges.
        /// for means of simplifying everything, this first implementation ignores
        /// the most simple effect, as it results in the more complex algorithm.
        ///
        /// DISREGARD EVERYTHING HERE BECAUSE I'M A LAZY DRUNKEN FOOL

        public int NoteChangeNumber; // the number of notes to change in the melody
        public int NumberOfBarChanges; // % NumberOfBarChanges is where we change the notes

        int[] noteChangeIndexes;
        int[] noteChangeOffsets;

        public ZMelodyImproviserAltNotes(ZMelody m, int noteChangeNumber, int targetBarNumber)
        {
            NoteChangeNumber = noteChangeNumber;
            NumberOfBarChanges = targetBarNumber;

            UpdateNoteChanges(m);
        }

        public override ZMelody Improvise(ZMelody melody, int currentBar)
        {
            for (int i = 0; i < melody.notes.Length; i++)
            {
                melody.notes[i] = GetNote(melody, i, currentBar);
            }
            return melody;
        }

        public ENote GetNote(ZMelody melody, int noteIndex, int currentBar)
        {
            // if we are not in the selected bar, just return the regular note
            if (currentBar % NumberOfBarChanges != 0) return melody.notes[noteIndex];

            // if this note is no supposed to change,
            // i.e., it is not in the noteIndexes-to-change array,
            // return a regular note
            if (!noteChangeIndexes.Contains(noteIndex)) return melody.notes[noteIndex];


            // otherwise, get the index of the current note in the array
            // and get the pre-defined offset
            int noteChangeIndex = Array.IndexOf(noteChangeIndexes, noteIndex);
            int offset = noteChangeOffsets[noteChangeIndex];

            ENote note = melody.notes[noteIndex];

            // then offset the note inside the scale by the pre-defined value
            var scale = Zapperz.GetScale(melody.scale, melody.root);


            int indexNoteInScale = Array.IndexOf(scale, note);

            ENote resultNote = scale[(indexNoteInScale + offset) % scale.Length];
            return resultNote;
        }

        private void UpdateNoteChanges(ZMelody m)
        {
            var scaleNotes = Zapperz.GetScale(m.scale, m.root);

            noteChangeIndexes = new int[Mathf.Min(NoteChangeNumber, m.notes.Length)];
            noteChangeIndexes[0] = 0;
            for (int i = 1; i < noteChangeIndexes.Length; i++)
            {
                // choose random index that's not been selected yet;
                int index = -1;
                do
                {
                    index = UnityEngine.Random.Range(0, m.notes.Length);
                } while (noteChangeIndexes.Contains(index));

                noteChangeIndexes[i] = index;
            }

            var scale = Zapperz.GetScale(m.scale, m.root);

            // select the offset of note changes
            noteChangeOffsets = new int[noteChangeIndexes.Length];
            for (int i = 0; i < noteChangeOffsets.Length; i++)
            {
                // choose the note offset;
                int noteOffset = UnityEngine.Random.Range(1, scale.Length); // choose a offset between 1 and an octave above (12)
                noteChangeOffsets[i] = noteOffset;
            }
        }
    }

    #endregion

    #region MARCH IMPROVISER 

    ////////////////////////////////////////////////////////////
    /// MARCH IMPROVISER 
    ///////////////////////////////////////////////////////////

    /**
     *  Generates note types for a bar of music
     * */
    public class ZMarchImproviser
    {
        public List<ZNoteTypeImproviser> Improvisers = new List<ZNoteTypeImproviser>();

        public bool HasImprovisers { get { return Improvisers.Count > 0; } }


        public ZMarch Improvise(ZMarch bar, int barIndex)
        {
            ZMarch m = new ZMarch(bar);
            foreach (var improviser in Improvisers)
            {
                m = improviser.Improvise(m, barIndex);
            }

            return m;
        }

        public void Add(ZNoteTypeImproviser improviser)
        {
            Improvisers.Add(improviser);
        }
    }

    public abstract class ZNoteTypeImproviser
    {
        public int TargetBar;
        public abstract ZMarch Improvise(ZMarch bar, int barIndex);

        public ZNoteTypeImproviser(int targetBarNumber)
        {
            TargetBar = targetBarNumber;
        }
    }

    /*
     * Fragments random notes into two sub notes.
     * A 4 (quarter note) becomes two 8s (eighth notes)
     * and so on.
     * */
    public class ZMarchFragmentImproviser : ZNoteTypeImproviser
    {
        public int[] indexes;

        public ZMarchFragmentImproviser(ZMarch m, int nNotesToFragment, int targetBarNumber) : base(targetBarNumber)
        {
            indexes = new int[Mathf.Min(nNotesToFragment, m.Size)];
            for (int i = 1; i < indexes.Length; i++)
            {
                // choose random index that's not been selected yet;
                int index = -1;
                do
                {
                    index = UnityEngine.Random.Range(0, m.Size);
                } while (indexes.Contains(index));

                indexes[i] = index;
            }
        }

        public override ZMarch Improvise(ZMarch bar, int barIndex)
        {
            if (barIndex % TargetBar != 0) return bar;


            int[] newBeats = new int[bar.Size + indexes.Length];

            for (int i = 0, j = 0; i < bar.Size; i++, j++)
            {
                int target = bar.Beats[i];
                if (indexes.Contains(i) && target < 32)
                {
                    newBeats[j] = ZMarchGenerator.BreakNote(target);
                    newBeats[j + 1] = ZMarchGenerator.BreakNote(target);
                    j++;
                }
                else
                {
                    newBeats[j] = bar.Beats[i];
                }
            }

            bar.Beats = newBeats;
            return bar;
        }
    }

    #endregion
}