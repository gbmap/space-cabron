using System.Collections.Generic;
using System.Linq;

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

        private bool ShouldApply(Melody melody, int barIndex, Note[] note, int noteIndex)
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
        NoteModifier modifierForDuplicates;

        public DuplicateNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=1,
            NoteModifier modifierForDuplicates=null
        ) : base(noteSelectionStrategy, barSelectionStrategy) 
        {
            TimesToDuplicate = System.Math.Max(0, timesToDuplicate+1);
            subNoteSelectionStrategy = new RandomSelectionStrategy();

            if (modifierForDuplicates == null)
                modifierForDuplicates = new NullNoteModifier();
            this.modifierForDuplicates = modifierForDuplicates;
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
                    Note n = notes[i].Copy();
                    notesList.AddRange(
                        Enumerable.Repeat(ApplyModifierToNote(n), TimesToDuplicate)
                    );
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

        protected Note ApplyModifierToNote(Note note)
        {
            return modifierForDuplicates.Modify(note);
        }
    }

    public class BreakNoteImprovisation : DuplicateNoteImprovisation
    {
        public BreakNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=2
        ) : base(noteSelectionStrategy, barSelectionStrategy, timesToDuplicate, new BreakNoteModifier(timesToDuplicate)) {}

        protected override string Info()
        {
            return $"Break note in {TimesToDuplicate} times";
        }
    }

    public class ApplyModifierImprovisation : Improvisation
    {
        protected NoteModifier modifier; 

        public ApplyModifierImprovisation(
            SelectionStrategy noteSelection,
            SelectionStrategy barSelection,
            NoteModifier modifier
        ) : base(noteSelection, barSelection) {
            this.modifier = modifier;
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            return notes.Select(n=> modifier.Modify(n)).ToArray();
        }

        protected override string Info()
        {
            return $"Apply modifier {modifier.ToString()}";
        }
    }

    public class TransposeNoteImprovisation : ApplyModifierImprovisation
    {
        public int Steps => (modifier as TransposeNoteModifier).Steps;
        public TransposeNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int steps=1
        ) : base(noteSelectionStrategy, barSelectionStrategy, new TransposeNoteModifier(steps)) 
        {}

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