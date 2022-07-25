using System.Collections.Generic;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class Improvisation
    {
        protected SelectionStrategy noteSelectionStrategy;
        protected SelectionStrategy barSelectionStrategy;
        public Improvisation(
            SelectionStrategy noteSelectionStrategy,
            SelectionStrategy barSelectionStrategy
        ) {
            this.noteSelectionStrategy = noteSelectionStrategy;
            this.barSelectionStrategy = barSelectionStrategy;
        }

        protected abstract Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex);
        protected abstract string Info();

        public bool ShouldApply(Melody melody, int barIndex, Note[] note, int noteIndex)
        {
            return noteSelectionStrategy.ShouldSelect(melody, noteIndex)
                && barSelectionStrategy.ShouldSelect(melody, barIndex);
        }


        public Note[] Apply(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            if (ShouldApply(melody, barIndex, notes, noteIndex))
                return ApplyImprovisation(melody, barIndex, notes, noteIndex);
            return notes;
        }

        public override string ToString()
        {
            return $"{Info()}:\n\tNote: {noteSelectionStrategy.ToString()}\n\tBar: {barSelectionStrategy.ToString()}";
        }
    }

    public class DuplicateNoteImprovisation : Improvisation
    {
        public int TimesToDuplicate { get; private set; }
        RandomSelectionStrategy subNoteSelectionStrategy;

        public DuplicateNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=1
        ) : base(noteSelectionStrategy, barSelectionStrategy) 
        {
            TimesToDuplicate = System.Math.Max(0, timesToDuplicate+1);
            subNoteSelectionStrategy = new RandomSelectionStrategy();
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            if (TimesToDuplicate == 0)
                return notes;

            List<Note> notesList = new List<Note>(notes.Length * (TimesToDuplicate));
            for (int i = 0; i < notes.Length; i++)
            {
                if (subNoteSelectionStrategy.ShouldSelect(notes.Length, i))
                {
                    Note n = new Note(notes[i]);
                    // n.Interval = n.Interval * TimesToDuplicate;
                    ChangeNote(n, noteIndex);
                    for (int j = 0; j < TimesToDuplicate; j++)
                        notesList.Add(n);
                }
                else
                    notesList.Add(notes[i]);
            }

            
            return notesList.ToArray();
        }

        protected override string Info()
        {
            return $"Duplicate {TimesToDuplicate}";
        }

        protected virtual void ChangeNote(Note note, int index)
        {
            // do nothing
        }
    }

    public class BreakNoteImprovisation : DuplicateNoteImprovisation
    {

        public BreakNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=2
        ) : base(noteSelectionStrategy, barSelectionStrategy, timesToDuplicate) {}

        protected override void ChangeNote(Note note, int index)
        {
            note.Interval *= TimesToDuplicate;
        }

        protected override string Info()
        {
            return $"Break note in {TimesToDuplicate} times";
        }
    }

    public class BreakNoteAndTranspose : DuplicateNoteImprovisation
    {
        public BreakNoteAndTranspose(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=2
        ) : base(noteSelectionStrategy, barSelectionStrategy, timesToDuplicate) {}

        protected override void ChangeNote(Note note, int index)
        {
            base.ChangeNote(note, index);
            note.Tone = (ENote)(note.Tone + UnityEngine.Random.Range(-12,12));
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

        protected override string Info()
        {
            return $"Transpose {Steps} steps";
        }
    }

    public class ReverseMelodyImprovisation : Improvisation
    {
        public ReverseMelodyImprovisation(SelectionStrategy barStrategy)
            : base(new EveryNStrategy(0), barStrategy)
        {}

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            return new Note[] {melody.GetNote(melody.Length-1-noteIndex)};
        }

        protected override string Info()
        {
            return "Revert melody";
        }
    }
}