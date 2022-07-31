using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface MelodyFactory
    {
        Melody Generate();
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

        public Melody Generate()
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

        public Melody Generate()
        {
            return new Melody(possibleMelodies.GetNext().Value);
        }
    }

    public class RandomMelodyFactory : MelodyFactory
    {
        ENote root;
        IScale scale;
        int numberOfNotes;

        public RandomMelodyFactory(ENote note, IScale scale, int numberOfNotes)
        {
            this.root = note;
            this.scale = scale;
            this.numberOfNotes = numberOfNotes;
        }

        public Melody Generate()
        {
            int[] intervals = new int[] { 2, 4, 8, 16 };
            Note[] noteArray = new Note[numberOfNotes];
            for (int i = 0; i< noteArray.Length; i++)
            {
                noteArray[i] = new Note(
                    scale.GetNote(root, Random.Range(0, scale.GetNumberOfNotes())), 
                    intervals[Random.Range(0, intervals.Length)], 
                    Random.Range(2, 8)
                );
            }
            return new Melody(noteArray);
        }
    }
}