using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * BEAT GENERATOR
 * Algorithms to generate beats.
 * 
 * */
public static class ZapperzFootTapping
{
    private static Dictionary<int, float> TimeFractions = new Dictionary<int, float>
    {
        { 1, 4f },      // whole note = beattime / 0.25 or *4
        { 2, 2f },      // half note = beattime / 0.5 or *2
        { 4, 1f },      // 4th note = beattime  / 1    or *1
        { 8, 0.5f },    // 8th note = beattime / 2  or * 0.5
        { 16, 0.25f },   // 16th note = beattime / 4  or * 0.25
        { 32, 0.125f }         // 32th note = beattime / 8  or * 0.125
    };

    /*
     * Fills a buffer of notes within a bar lazily.
     * First note = Random whole, half, quarter, ...
     * Second note = Random note that is < Total bar time - time consumed until now.
     * Third note = ...
     * N note = ...
     * */
    public static ZBar GenerateProcrastinatorBeat(int BPM, // beats per minute
                                                   int notesInBar, // upper number of time signature
                                                   int noteType, // lower number of time signature
                                                   Vector2Int noteRange) 
    {
        float bps = BPMtoBPS(BPM);
        float qbt = GetQuarterBeatTime(BPM); // quarter beat time

        int beatType = noteType;
        float nt = TimeFractions[beatType];
        float barTime = qbt * nt * notesInBar;

        // list of whole, half, quarter beats that fills the bar
        List<int> beatTypes = new List<int>();
        float totalTime = 0f;
        float targetTime = barTime;
        while (totalTime < targetTime)
        {
            int attempts = 0;
            float randomTime = float.MaxValue;
            int randomNote = -1;
            while (totalTime + randomTime > targetTime)
            {
                randomNote = 1 << UnityEngine.Random.Range(noteRange.x, noteRange.y);
                randomTime = GetBeatType(BPM, randomNote);
                attempts++;

                if (attempts > 256) throw new System.Exception("Infinite loop.");
            }

            totalTime += randomTime;
            beatTypes.Add(randomNote);
        }

        return new ZBar(beatTypes.ToArray());
    }

    ///////////////////
    /// UTILITY
    /// 
    public static float BPMtoBPS(int BPM)
    {
        return (float)BPM / 60;
    }

    // quarter beat time
    public static float GetQuarterBeatTime(int BPM)
    {
        return (float)60 / BPM;
    }

    public static float GetBeatType(int BPM, int noteType)
    {
        return GetQuarterBeatTime(BPM) * TimeFractions[noteType];
    }

    public static float GetBarTime(int BPM, int notesInBar, int noteType)
    {
        float bps = BPMtoBPS(BPM);
        float qbt = GetQuarterBeatTime(BPM); // quarter beat time

        float nt = TimeFractions[noteType];
        return (qbt*nt) * notesInBar;
    }

    
    
}

[System.Serializable]
public class ZBar
{
    //public float[] Beats;
    public int[] Beats;
    
    public int Cursor = 0;
    public int CurrentBeat
    {
        get { return Beats[Cursor]; }
    }

    public ZBar(int[] noteTypes)
    {
        Beats = noteTypes;
    }

    // returns the current beat type
    public int Advance(out bool beatEnded)
    {
        int beatType = Beats[Cursor];
        Cursor = (Cursor + 1) % Beats.Length;
        beatEnded = Cursor == 0;
        return beatType;
    }
}