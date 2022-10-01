using System.Linq;
using Gmap.ScriptableReferences;
using Gmap.Utils;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface MelodyFactory
    {
        Melody GenerateMelody();
    }

    public interface INoteArrayFactory
    {
        Note[] GenerateNotes(int[] intervals);
    }

    public interface IIntervalsFactory
    {
        int[] GenerateIntervals(int n);
    }

    public class FixedMelodyFactory : MelodyFactory
    {
        Melody melody;
        public FixedMelodyFactory(string notation)
        {
            this.melody = new Melody(notation);
        }

        public FixedMelodyFactory(Melody m)
        {
            this.melody = m;
        }

        public Melody GenerateMelody()
        {
            return this.melody;
        }
    }

    public class StringPoolMelodyFactory : MelodyFactory
    {
        StringReferencePool possibleMelodies;

        public StringPoolMelodyFactory(StringReferencePool pool)
        {
            possibleMelodies = pool;
        }

        public Melody GenerateMelody()
        {
            return new Melody(possibleMelodies.GetNext().Value);
        }
    }

    public class RandomMelodyFactory : MelodyFactory
    {
        ENote root;
        IScale scale;
        int numberOfNotes;
        IntReference Octave;

        public RandomMelodyFactory(ENote note,
                                   IScale scale,
                                   int numberOfNotes,
                                   IntReference octave
        ) {
            this.root = note;
            this.scale = scale;
            this.numberOfNotes = numberOfNotes;
            this.Octave = octave;
        }

        public Melody GenerateMelody()
        {
            Debug.Log($"Generating melody with config:\n\tRoot: {root}\n\tScale: {scale}\n\tNumber of notes: {numberOfNotes}\n\tOctave {Octave}");
            int[] intervals = new int[] { 4, 8, 16 };
            Note[] noteArray = new Note[numberOfNotes];
            for (int i = 0; i< noteArray.Length; i++)
            {
                noteArray[i] = new Note(
                    scale.GetNote(root, Random.Range(0, scale.GetNumberOfNotes())), 
                    intervals[Random.Range(0, intervals.Length)], 
                    Octave.Value
                );
            }
            Melody melody = new Melody(noteArray); 
            melody.Root = root;
            melody.Scale = scale;
            return melody;
        }
    }

    public class BreakNoteMelodyFactory : MelodyFactory
    {
        ENote root;
        IScale scale;
        IntReference octave;
        Vector2Int timeSignature;
        int numberOfBreaks;
        public BreakNoteMelodyFactory(
            ENote root,
            IScale scale,
            IntReference octave,
            Vector2Int timeSignature,
            int numberOfBreaks
        ) {
            this.root = root;
            this.scale = scale;
            this.timeSignature = timeSignature;
            this.octave = octave;
            this.numberOfBreaks = numberOfBreaks;
        }

        public Melody GenerateMelody()
        {
            int[] intervals = Enumerable.Range(0, timeSignature.x)
                      .Select(i=>timeSignature.y)
                      .ToArray();

            ShuffleBag<int> steps = new ShuffleBag<int>();
            steps.Add(-1, 5);
            steps.Add(1, 5);
            steps.Add(-2, 3);
            steps.Add(2, 3);
            steps.Add(-3, 2);
            steps.Add(3, 2);
            steps.Add(-4, 1);
            steps.Add(4, 1);

            ENote[] tones = new ENote[timeSignature.x];
            ENote currentTone = scale.GetNote(root, 0);
            int currentIndex = 0;
            for (int i = 0; i < timeSignature.x; i++) {
                tones[i] = currentTone;
                currentIndex += steps.Next();
                currentTone = scale.GetNote(root, currentIndex);
            }

            Note[] notes = Enumerable.Range(0, timeSignature.x)
                      .Select(i=>new Note(
                          tones[i],
                          intervals[i],
                          octave.Value
                      ))
                      .ToArray();

            Melody m = new Melody(notes);
            for (int i = 0; i < numberOfBreaks; i++)
            {
                int index = Random.Range(0, m.Length);
                m = new BreakMelodyNoteModifier(2).Apply(m);
            }

            return m;
        }

        protected ENote GetRandomNote()
        {
            return this.scale.GetNote(
                this.root,
                Random.Range(0, this.scale.GetNumberOfNotes())
            );
        }
    }

    public class RandomImprovisationMelodyFactory : MelodyFactory
    {
        ENote root;
        IScale scale;
        IntReference octave;
        Vector2Int timeSignature;
        int numberOfImprovisations;
        ImprovisationPool improvisations;
        int numberOfBars;
        int numberOfShuffles;

        public RandomImprovisationMelodyFactory(
            ENote root,
            IScale scale,
            IntReference octave,
            Vector2Int timeSignature,
            int numberOfImprovisations,
            ImprovisationPool improvisations,
            int numberOfBars,
            int numberOfShuffles
        ) {
            this.root = root;
            this.scale = scale;
            this.timeSignature = timeSignature;
            this.octave = octave;
            this.numberOfImprovisations = numberOfImprovisations;
            this.improvisations = improvisations;
            this.numberOfBars = numberOfBars;
            this.numberOfShuffles = numberOfShuffles;
        }

        public Melody GenerateMelody()
        {
            int[] intervals = Enumerable.Range(0, timeSignature.x*numberOfBars)
                      .Select(i=>timeSignature.y)
                      .ToArray();

            // ENote[] tones = Enumerable.Range(0, timeSignature.x*numberOfBars)
            //                        .Select(i=>GetRandomNote())
            //                        .ToArray();     
            // int octave = this.octave.Value;

            ShuffleBag<int> steps = new ShuffleBag<int>();
            steps.Add(-1, 5);
            steps.Add(1, 5);
            steps.Add(-2, 3);
            steps.Add(2, 3);
            steps.Add(-3, 2);
            steps.Add(3, 2);
            steps.Add(-4, 1);
            steps.Add(4, 1);
            steps.Add(-5, 1);
            steps.Add(5, 1);
            steps.Add(-6, 1);
            steps.Add(6, 1);
            steps.Add(-7, 1);
            steps.Add(7, 1);

            ENote[] tones = new ENote[timeSignature.x*numberOfBars];
            ENote currentTone = scale.GetNote(root, 0);
            int currentIndex = 0;
            for (int i = 0; i < timeSignature.x; i++) {
                tones[i] = currentTone;
                currentIndex += steps.Next();
                currentTone = scale.GetNote(root, currentIndex);
            }

            Note[] notes = Enumerable.Range(0, timeSignature.x*numberOfBars)
                      .Select(i=>new Note(
                          tones[i],
                          intervals[i],
                          octave.Value
                      ))
                      .ToArray();

            Melody m = new Melody(notes);
            for (int i = 0; i < numberOfImprovisations; i++)
            {
                int index = Random.Range(0, m.Length);
                var improvisation = improvisations.GetNext().Get();
                improvisation.NoteSelectionStrategy = new RangeStrategy(index, index);

                // Improvisations use subnoteselection to apply improvisations 
                // on notes generated previously by other improvisations,
                // but, since we are trying to apply <improvisation> to an specific
                // note, we need to set the subnote selection strategy to the same strategy.
                improvisation.SubNoteSelectionStrategy = improvisation.NoteSelectionStrategy;
                m = new Melody(improvisation.Apply(m, 0, m.NoteArray, index));
            }

            if (m.NoteArray.Length == 1)
                return m;

            var notesShuffled = m.NoteArray;
            for (int i = 0; i < numberOfShuffles; i++) {
                int i1 = Random.Range(0, notesShuffled.Length);
                int i2 = Random.Range(0, notesShuffled.Length);
                while (i1 == i2) {
                    i2 = Random.Range(0, notesShuffled.Length);
                }

                var temp = notesShuffled[i1];
                notesShuffled[i1] = notesShuffled[i2];
                notesShuffled[i2] = temp;
            }

            m = new Melody(notesShuffled);
            return m;
        }

        protected ENote GetRandomNote()
        {
            return this.scale.GetNote(
                this.root,
                Random.Range(0, this.scale.GetNumberOfNotes())
            );
        }
    }

    public abstract class MelodyFactoryBase : MelodyFactory, INoteArrayFactory, IIntervalsFactory
    {
        protected int numberOfNotes;
        protected int[] intervals;
        protected Note[] notes;
        protected IntReference octave;

        protected ENote root;
        protected Scale scale;

        public MelodyFactoryBase(ENote root, Scale scale, int numberOfNotes, IntReference octave)
        {
            this.root = root;
            this.scale = scale;
            this.numberOfNotes = numberOfNotes;
            this.octave = octave;
        }

        public Melody GenerateMelody()
        {
            intervals = GenerateIntervals(numberOfNotes);
            notes = GenerateNotes(intervals);
            return new Melody(notes);
        }

        public abstract Note[] GenerateNotes(int[] intervals);
        public abstract int[] GenerateIntervals(int n);
    }

    public class ShuffleBagMelodyFactory : MelodyFactoryBase
    {
        ShuffleBag<ENote> noteBag;
        public ShuffleBagMelodyFactory(ENote root, Scale scale, int numberOfNotes, IntReference octave) 
            : base(root, scale, numberOfNotes, octave)
        {
            noteBag = new ShuffleBag<ENote>();

            noteBag.Add(scale.GetNote(root, 0), 10);
            if (scale.GetNumberOfNotes() >= 4)
                noteBag.Add(scale.GetNote(root, 3), 9);
            if (scale.GetNumberOfNotes() >= 5)
                noteBag.Add(scale.GetNote(root, 4), 8);

            int[] ignoreIndexes = new int[] { 0, 3, 4 };
            for (int i = 0; i < scale.GetNumberOfNotes(); i++)
            {
                if (ignoreIndexes.Contains(i))
                    continue;

                noteBag.Add(scale.GetNote(root, i), 3);
            }
        }

        public override Note[] GenerateNotes(int[] intervals)
        {
            Note[] notes = new Note[intervals.Length];
            intervals.Select(i => new Note
            {
                Interval = i,
                Octave = octave.Value,
                Tone = noteBag.Next()
            });
            return notes;
        }
        public override int[] GenerateIntervals(int n)
        {
            return Enumerable.Range(0, n).Select(i=>2<<Random.Range(0,3)).ToArray();
        }
    }

}