using System.Collections.Generic;
using System.Linq;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class Improvisation
    {
        protected SelectionStrategy noteSelectionStrategy;
        protected SelectionStrategy barSelectionStrategy;
        protected SelectionStrategy subNoteSelectionStrategy;
        public Improvisation(
            SelectionStrategy noteSelectionStrategy,
            SelectionStrategy barSelectionStrategy,
            SelectionStrategy subNoteSelectionStrategy=null
        ) {
            this.noteSelectionStrategy = noteSelectionStrategy;
            this.barSelectionStrategy = barSelectionStrategy;
            if (subNoteSelectionStrategy == null)
                subNoteSelectionStrategy = new RandomSelectionStrategy();
            this.subNoteSelectionStrategy = subNoteSelectionStrategy;
        }

        protected abstract Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex);
        protected abstract string Info();

        private bool ShouldApply(Melody melody, int barIndex, Note[] note, int noteIndex)
        {
            return noteSelectionStrategy.ShouldSelect(melody.NoteArray, noteIndex)
                && barSelectionStrategy.ShouldSelect(melody.NoteArray, barIndex);
        }


        public Note[] Apply(Melody melody, int barIndex, Note[] notes, int noteIndex)
        {
            if (ShouldApply(melody, barIndex, notes, noteIndex))
            {
                List<Note> noteList = new List<Note>();
                for (int i = 0; i < notes.Length; i++)
                {
                    if (subNoteSelectionStrategy.ShouldSelect(notes, i))
                    {
                        Note[] newNotes = ApplyImprovisation(melody, barIndex, notes[i], noteIndex);
                        noteList.AddRange(newNotes);
                    }
                    else
                        noteList.Add(notes[i]);
                }
                return noteList.ToArray();
            }
            
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
        NoteModifier modifierForDuplicates;
        SelectionStrategy duplicateSelectionStrategy;

        public DuplicateNoteImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int timesToDuplicate=1,
            NoteModifier modifierForDuplicates=null,
            SelectionStrategy duplicateSelectionStrategy = null
        ) : base(noteSelectionStrategy, barSelectionStrategy) 
        {
            TimesToDuplicate = System.Math.Max(0, timesToDuplicate+1);
            subNoteSelectionStrategy = new RandomSelectionStrategy();

            if (modifierForDuplicates == null)
                modifierForDuplicates = new NullNoteModifier();
            this.modifierForDuplicates = modifierForDuplicates;

            if (duplicateSelectionStrategy == null)
                duplicateSelectionStrategy = new SelectAllStrategy();
            this.duplicateSelectionStrategy = duplicateSelectionStrategy;
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex)
        {
            if (TimesToDuplicate == 0)
                return new Note[] { note };

            List<Note> notes = new List<Note>();

            for (int i = 0; i < TimesToDuplicate; i++)
            {
                if (duplicateSelectionStrategy.ShouldSelect(notes.ToArray(), i))
                    notes.Add(modifierForDuplicates.Modify(new Note(note)));
                else
                    notes.Add(note);
            }
            return notes.ToArray();
        }

        protected override string Info()
        {
            return $"Duplicate {TimesToDuplicate}";
        }

        protected virtual Note ApplyModifierToNote(Note note, int i)
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
        ) : base(noteSelectionStrategy, barSelectionStrategy, timesToDuplicate, new IncreaseIntervalNoteModifier(timesToDuplicate)) {}

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

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex)
        {
            return new Note[] { modifier.Modify(note) };
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

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note notes, int noteIndex)
        {
            return new Note[] {melody.GetNote(melody.Length-1-noteIndex)};
        }

        protected override string Info()
        {
            return "Revert melody";
        }
    }

    public class UpperMordentImprovisation : DuplicateNoteImprovisation
    {
        public UpperMordentImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy
        ) : base(
            noteSelectionStrategy, 
            barSelectionStrategy, 
            2, 
            new TransposeNoteModifier(1),
            new EveryNStrategy(2)) {}

        protected override string Info()
        {
            return "Upper mordent.";
        }
    }
}