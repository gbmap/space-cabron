using System.Collections.Generic;
using UnityEngine;

public enum EScale
{
    /* 
     * 
     * https://lotusmusic.com/lm_exoticscales.html
     * 
     */

    

    // PENTATONIC
    MajorPentatonic,
    MinorPentatonic,
    Japanese,

    // STANDARD
    Major,
    Minor,

    // MODES
    Ionian,
    Dorian,
    Phrygian,
    Lydian,
    Mixolydian,
    Aeolian,
    Locrian,
    PhrygianDominant,

    // SEVEN TONE MAJOR
    Persian,
    HungarianMinor,

    // SIX TONE
    Mystic,
    
    // EIGHT TONE
    Spanish8,




    Custom = 99999
}

public class Zapperz
{
    public static ENote[] GetScale(EScale scale, ENote n)
    {
        return GenerateScale(n, GetScaleSteps(scale));
    }

    public static ENote[] GenerateScale(ENote n, int[] steps)
    {
        return GenerateScale(n, steps, new List<ENote>()).ToArray();
    }

    public static ENote OffsetNote(ENote n, int steps)
    {
        return (ENote)(((int)n + steps) % 12);
    }

    public static List<ENote> GenerateScale(ENote n, int[] steps, List<ENote> notes, int i = 0)
    {
        if (i == steps.Length)
            return notes;

        notes.Add(n);
        return GenerateScale(OffsetNote(n, steps[i]), steps, notes, i+1);
    }

    private static int[] GetScaleSteps(EScale scale)
    {
        switch (scale)
        {
            default:

            // PENTATONICS
            case EScale.MajorPentatonic:
                return new int[] { 2, 2, 3, 2, 3 };
            case EScale.MinorPentatonic:
                return new int[] { 3, 2, 2, 3, 2 };
            case EScale.Japanese:
                return new int[] { 2, 1, 4, 1, 4 };

            // MODES
            case EScale.Minor:
            case EScale.Aeolian:
                // W H W W H W W
                return new int[] { 2, 1, 2, 2, 1, 2, 2 };
            case EScale.PhrygianDominant:
                // H + H W H W W 
                return new int[] { 1, 3, 1, 2, 1, 2, 2 };

            case EScale.Major:
            case EScale.Ionian:
                // W W H W W W H
                return new int[] { 2, 2, 1, 2, 2, 2, 1 };
            case EScale.Dorian:
                // W H W W W H W
                return new int[] { 2, 1, 2, 2, 2, 1, 1 };
            case EScale.Phrygian:
                // H W W W H W W
                return new int[] { 1, 2, 2, 2, 1, 2, 2 };
            case EScale.Lydian:
                // W W W H W W H
                return new int[] { 2, 2, 2, 1, 2, 2, 1 };
            case EScale.Mixolydian:
                // W W H W W H W
                return new int[] { 2, 2, 1, 2, 2, 1, 2 };
            case EScale.Locrian:
                // H W W H W W W 
                return new int[] { 1, 2, 2, 1, 2, 2, 2 };

            // MISC 

            // SEVEN TONE MAJOR SCALES
            case EScale.Persian:
                // H + H H W + H
                return new int[] { 1, 3, 1, 1, 2, 3, 1 };

            // SEVEN TONE MINOR SCALES
            case EScale.HungarianMinor:
                // W H + H H + H
                return new int[] { 2, 1, 3, 1, 1, 3, 1 };

            // SIX TONE SCALES
            case EScale.Mystic:
                // W W W + W W
                return new int[] { 2, 2, 2, 3, 2, 2 };

            // 8 TONE
            case EScale.Spanish8:
                return new int[] { 1, 2, 1, 1, 1, 2, 2, 2 };
        }
    }

}
