using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
            // // PENTATONICS
            // case EScale.MajorPentatonic:
            //     return new int[] { 2, 2, 3, 2, 3 };
            // case EScale.MinorPentatonic:
            //     return new int[] { 3, 2, 2, 3, 2 };
            // case EScale.Japanese:
            //     return new int[] { 2, 1, 4, 1, 4 };

            // // MODES
            // case EScale.Minor:
            // case EScale.Aeolian:
            //     // W H W W H W W
            //     return new int[] { 2, 1, 2, 2, 1, 2, 2 };
            // case EScale.PhrygianDominant:
            //     // H + H W H W W 
            //     return new int[] { 1, 3, 1, 2, 1, 2, 2 };

            // case EScale.Major:
            // case EScale.Ionian:
            //     // W W H W W W H
            //     return new int[] { 2, 2, 1, 2, 2, 2, 1 };
            // case EScale.Dorian:
            //     // W H W W W H W
            //     return new int[] { 2, 1, 2, 2, 2, 1, 1 };
            // case EScale.Phrygian:
            //     // H W W W H W W
            //     return new int[] { 1, 2, 2, 2, 1, 2, 2 };
            // case EScale.Lydian:
            //     // W W W H W W H
            //     return new int[] { 2, 2, 2, 1, 2, 2, 1 };
            // case EScale.Mixolydian:
            //     // W W H W W H W
            //     return new int[] { 2, 2, 1, 2, 2, 1, 2 };
            // case EScale.Locrian:
            //     // H W W H W W W 
            //     return new int[] { 1, 2, 2, 1, 2, 2, 2 };

            // // MISC 

            // // SEVEN TONE MAJOR SCALES
            // case EScale.Persian:
            //     // H + H H W + H
            //     return new int[] { 1, 3, 1, 1, 2, 3, 1 };

            // // SEVEN TONE MINOR SCALES
            // case EScale.HungarianMinor:
            //     // W H + H H + H
            //     return new int[] { 2, 1, 3, 1, 1, 3, 1 };

            // // SIX TONE SCALES
            // case EScale.Mystic:
            //     // W W W + W W
            //     return new int[] { 2, 2, 2, 3, 2, 2 };

            // // 8 TONE
            // case EScale.Spanish8:
            //     return new int[] { 1, 2, 1, 1, 1, 2, 2, 2 };
    
    public interface IScale
    {
        public ENote GetNote(ENote root, int i);
        public int GetNumberOfNotes();
    }

    [System.Serializable]
    public class Scale : IScale
    {
        public int[] Intervals;

        public ENote GetNote(ENote root, int i)
        {
            if (Intervals.Length == 0)
                return ENote.None;

            i = Bar.MathMod(i,Intervals.Length);
            return Note.OffsetNote(root, Intervals[..i].Sum(x=>x));
        }

        public int GetNumberOfNotes()
        {
            return Intervals.Length;
        }
    }
}