using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public virtual bool ShouldApply(Melody melody, int barIndex, Note[] note, int noteIndex)
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

        protected override Note[] ApplyImprovisation(
            Melody melody, 
            int barIndex, 
            Note note, 
            int noteIndex
        ) {
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
        ) : base(noteSelectionStrategy, 
        barSelectionStrategy, 
        timesToDuplicate, 
        new IncreaseIntervalNoteModifier(timesToDuplicate)
        ) {}

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

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex)
        {
            Note n = new Note(note);
            if (melody.Scale != null && melody.Root != ENote.None)
            {
                int scaleIndex = melody.Scale.GetIndex(melody.Root, n.Tone);
                n.Tone = melody.Scale.GetNote(melody.Root, scaleIndex+Steps);
            }
            else
                n = Note.TransposeWrapped(note, Steps);
            return new Note[] { n };
        }

        protected override string Info()
        {
            return "Transpose note " + Steps + " steps.";
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

    public class MordentImprovisation : Improvisation
    {
        int Steps;
        public MordentImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int steps=1
        ) : base(
            noteSelectionStrategy, 
            barSelectionStrategy
        ) {
            Steps = steps;
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex)
        {
            Note n1 = new Note(note);
            n1.Interval *= 4;

            Note n2 = new Note(n1);
            n2.TransposeWrapped(Steps);

            Note n3 = new Note(note);
            n3.Interval *= 2;

            return new Note[] { n1, n2, n3 };
        }

        protected override string Info()
        {
            return $"Upper mordent {Steps} steps.";
        }
    }

    public class TremoloImprovisation : Improvisation
    {
        int Steps; 
        int Beams;

        public TremoloImprovisation(
            SelectionStrategy noteSelectionStrategy, 
            SelectionStrategy barSelectionStrategy,
            int beams,
            int steps=1

        ) : base(noteSelectionStrategy, barSelectionStrategy) {
            Steps = steps;
            Beams = beams;
        }

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex)
        {
            int numberOfNotes = (int)Mathf.Pow(2, Beams);
            int newInterval = note.Interval * numberOfNotes;
            Note n = new Note(note);
            n.Interval = newInterval;
            Note[] notes = new Note[numberOfNotes];
            for (int i = 0; i < numberOfNotes; i++)
            {
                notes[i] = new Note(n);
                if (i % 2 != 0)
                    continue;
                notes[i].TransposeWrapped(Steps);
            }
            return notes;
        }

        protected override string Info()
        {
            return "Tremolo.";
        }
    }

    public class ResolveNoteImprovisation : Improvisation
    {
        public ResolveNoteImprovisation(
            SelectionStrategy barSelectionStrategy
        ) : base(new SelectAllStrategy(), barSelectionStrategy) {}

        protected override Note[] ApplyImprovisation(Melody melody, int barIndex, Note note, int noteIndex)
        {
            if (melody.Root == ENote.None)
                return new Note[] { note };

            Note longestNote = melody.NoteArray.OrderBy(n=>n.Interval).First();
            if (longestNote != note)
                return new Note[] { note };
            
            Note n = new Note(note);
            n.Tone = melody.Root;
            return new Note[] { n };
        }

        protected override string Info()
        {
            return "Resolve note to root.";
        }
    }
}