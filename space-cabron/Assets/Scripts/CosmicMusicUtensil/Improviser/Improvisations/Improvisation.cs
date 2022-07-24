using System.Collections.Generic;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class Improvisation
    {
        SelectionStrategy noteSelectionStrategy;
        SelectionStrategy barSelectionStrategy;
        public Improvisation(
            SelectionStrategy noteSelectionStrategy,
            SelectionStrategy barSelectionStrategy
        ) {
            this.noteSelectionStrategy = noteSelectionStrategy;
            this.barSelectionStrategy = barSelectionStrategy;
        }

        public bool ShouldApply(Melody melody, int barIndex, Note[] note, int noteIndex)
        {
            return noteSelectionStrategy.ShouldSelect(melody, noteIndex)
                && barSelectionStrategy.ShouldSelect(melody, barIndex);
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
        public int TimesToDuplicate { get; private set; }

        public DuplicateNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=2
        ) : base(noteSelectionStrategy, barSelectionStrategy) 
        {
            TimesToDuplicate = timesToDuplicate;
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            List<Note> notesList = new List<Note>(notes.Length * TimesToDuplicate);
            for (int i = 0; i < TimesToDuplicate; i++)
                notesList.AddRange(notes);
            return notesList.ToArray();
        }
    }

    public class TransposeNoteImprovisation : Improvisation
    {
        public int Steps { get; private set; }
        public TransposeNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int steps=1
        ) : base(noteSelectionStrategy, barSelectionStrategy) 
        {
            Steps = steps;
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            Note[] newNotes = new Note[notes.Length];
            for (int i = 0; i < notes.Length; i++)
                newNotes[i] = Note.Transpose(notes[i], Steps);
            return notes;
        }
    }
}