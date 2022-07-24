using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class Improvisation
    {
        NoteSelectionStrategy selectionStrategy;
        public Improvisation(NoteSelectionStrategy selectionStrategy)
        {
            this.selectionStrategy = selectionStrategy;
        }

        public bool ShouldApply(Melody melody, int barIndex, Note[] note, int noteIndex)
        {
            return selectionStrategy.ShouldSelectNote(melody, barIndex, noteIndex);
        }

        protected abstract Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex);

        public Note[] Apply(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            if (ShouldApply(melody, barIndex, notes, noteIndex))
                return ApplyImprovisation(melody, barIndex, notes, noteIndex);
            return notes;
        }
    }

    public class DuplicateNoteImprovisation : Improvisation
    {
        public DuplicateNoteImprovisation(NoteSelectionStrategy strategy)
            : base(strategy)
        {}

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            Note[] finalNotes = new Note[notes.Length * 2];
            for (int i = 0; i < notes.Length; i++)
            {
                finalNotes[i] = notes[i];
                finalNotes[i+1] = notes[i];
            }
            return finalNotes;
        }
    }
}