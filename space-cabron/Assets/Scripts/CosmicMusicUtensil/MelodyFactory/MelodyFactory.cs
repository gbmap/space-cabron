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

        public RandomMelodyFactory(ENote note, IScale scale, int numberOfNotes, IntReference octave)
        {
            this.root = note;
            this.scale = scale;
            this.numberOfNotes = numberOfNotes;
            this.Octave = octave;
        }

        public Melody GenerateMelody()
        {
            Debug.Log($"Generating melody with config:\n\tRoot: {root}\n\tScale: {scale}\n\tNumber of notes: {numberOfNotes}\n\tOctave {Octave}");
            int[] intervals = new int[] { 2, 4, 8, 16 };
            Note[] noteArray = new Note[numberOfNotes];
            for (int i = 0; i< noteArray.Length; i++)
            {
                noteArray[i] = new Note(
                    scale.GetNote(root, Random.Range(0, scale.GetNumberOfNotes())), 
                    intervals[Random.Range(0, intervals.Length)], 
                    Octave.Value
                );
            }
            return new Melody(noteArray);
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