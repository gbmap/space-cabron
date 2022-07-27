using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface SelectionStrategy
    {
        bool ShouldSelect(Melody melody, int index);
    }

    public class SelectAllStrategy : SelectionStrategy
    {
        public bool ShouldSelect(Melody melody, int index)
        {
            return true;
        }
    }


    public class EveryNStrategy : SelectionStrategy
    {
        public int N { get; set; }
        public EveryNStrategy(int n)
        {
            N = Mathf.Max(1, n);
        }

        public bool ShouldSelect(Melody melody, int index)
        {
            return ((index+1) % N) == 0;
        }

        public override string ToString()
        {
            return "Every " + N;
        }
    }

    public class RangeStrategy : SelectionStrategy
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public RangeStrategy(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool ShouldSelect(Melody melody, int index)
        {
            return index >= Min && index <= Max;
        }

        public override string ToString()
        {
            return "Range " + Min + "-" + Max;
        }
    }

    public class RandomSelectionStrategy : SelectionStrategy
    {
        int seed;
        public RandomSelectionStrategy()
        {
            seed = Random.Range(0, 100);
        }

        public bool ShouldSelect(Melody melody, int index)
        {
            return ShouldSelect(melody.Length, index);
        }

        public bool ShouldSelect(int length, int index)
        {
            var r = new System.Random(seed);
            return index == r.Next(0, length);
        }
    }

    public class CompositeNoteSelectionStrategy : SelectionStrategy
    {
        public enum EOperation
        {
            AND,
            OR,
            XOR,
        }
        public EOperation Operation { get; set; }
        public List<SelectionStrategy> Strategies { get; set; }
        public CompositeNoteSelectionStrategy(EOperation operation=EOperation.AND)
        {
            Strategies = new List<SelectionStrategy>();
            Operation = operation;
        }

        public CompositeNoteSelectionStrategy(
            List<SelectionStrategy> strategies, 
            EOperation operation=EOperation.AND
        ) {
            Strategies = strategies;
            Operation = operation;
        }

        public bool ShouldSelect(Melody melody, int index)
        {
            switch (Operation)
            {
                case EOperation.AND:
                    return Strategies.TrueForAll(s => s.ShouldSelect(melody, index));
                case EOperation.OR:
                    return Strategies.Exists(s => s.ShouldSelect(melody, index));
                case EOperation.XOR:
                    return Strategies.Exists(s => s.ShouldSelect(melody, index)) &&
                        !Strategies.TrueForAll(s => s.ShouldSelect(melody, index));
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return "Composite " + Operation + "[" + string.Join(", ", Strategies) + "]";
        }
    }
}