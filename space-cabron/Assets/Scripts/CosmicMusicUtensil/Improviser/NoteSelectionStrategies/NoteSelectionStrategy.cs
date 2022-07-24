using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface NoteSelectionStrategy
    {
        bool ShouldSelectNote(Melody melody, int barIndex, int noteIndex);
    }

    public class EveryNBarStrategy : NoteSelectionStrategy
    {
        public int N { get; set; }
        public EveryNBarStrategy(int n)
        {
            N = Mathf.Max(1, n);
        }

        public bool ShouldSelectNote(Melody melody, int barIndex, int noteIndex)
        {
            return ((barIndex+1) % N) == 0;
        }
    }

    public class EveryNNoteStrategy : NoteSelectionStrategy
    {
        public int N { get; set; }
        public EveryNNoteStrategy(int n)
        {
            N = Mathf.Max(1,n);
        }

        public bool ShouldSelectNote(Melody melody, int barIndex, int noteIndex)
        {
            return ((noteIndex+1) % N) == 0;
        }
    }

    public class CompositeNoteSelectionStrategy : NoteSelectionStrategy
    {
        public enum EOperation
        {
            AND,
            OR,
            XOR,
        }
        public EOperation Operation { get; set; }
        public List<NoteSelectionStrategy> Strategies { get; set; }
        public CompositeNoteSelectionStrategy(EOperation operation=EOperation.AND)
        {
            Strategies = new List<NoteSelectionStrategy>();
            Operation = operation;
        }

        public CompositeNoteSelectionStrategy(
            List<NoteSelectionStrategy> strategies, 
            EOperation operation=EOperation.AND
        ) {
            Strategies = strategies;
            Operation = operation;
        }

        public bool ShouldSelectNote(Melody melody, int barIndex, int noteIndex)
        {
            switch (Operation)
            {
                case EOperation.AND:
                    return Strategies.TrueForAll(s => s.ShouldSelectNote(melody, barIndex, noteIndex));
                case EOperation.OR:
                    return Strategies.Exists(s => s.ShouldSelectNote(melody, barIndex, noteIndex));
                case EOperation.XOR:
                    return Strategies.Exists(s => s.ShouldSelectNote(melody, barIndex, noteIndex)) &&
                        !Strategies.TrueForAll(s => s.ShouldSelectNote(melody, barIndex, noteIndex));
                default:
                    return false;
            }
        }
    }
}