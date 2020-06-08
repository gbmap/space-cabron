using System.Linq;
using UnityEngine;
using Utils;

public class Beat
{
    public int SubBeats;
    public int Index;

    public bool Advance()
    {
        Index++;
        if (Index == SubBeats)
        {
            Index = 0;
            return true;
        }
        return false;
    }

    public Beat(int subBeats)
    {
        SubBeats = subBeats;
        Index = 0;
    }
}

public class Loop
{
    public Beat[] Beats;
    public int Index;

    public Beat CurrentBeat { get { return Beats[Index]; } }
    public Beat LastBeat { get { return Beats[Mathf.Max(0, Index - 1)]; } }

    public int BeatCount
    {
        get
        {
            return Beats.Sum(b => b.SubBeats);
        }
    }

    public bool Advance(out bool endBeat)
    {
        endBeat = CurrentBeat.Advance();
        if (endBeat)
        {
            Index++;
            if (Index == Beats.Length)
            {
                Index = 0;
                return true;
            }
        }
        return false;
    }
}

public class LoopCreator
{
    public const int MAX_LOOPBEATS = 16;

    public int NBeats = 8;
    public int MaxSubBeats = 1;

    public LoopCreator(int nBeats, int maxSubBeats)
    {
        NBeats = nBeats;
        MaxSubBeats = maxSubBeats;
    }

    public static int RandomSubBeats(int MaxSubBeats)
    {
        return Mathf.FloorToInt(1+Mathf.RoundToInt(Mathf.Round(Mathf.Pow(UnityEngine.Random.value, 1f)*MaxSubBeats) % MaxSubBeats));
    }

    public Loop Create()
    {
        Loop l = new Loop();
        l.Beats = new Beat[NBeats];

        for (int i = 0; i < NBeats; i++)
            l.Beats[i] = new Beat(RandomSubBeats(MaxSubBeats));

        return l;
    }

    public static Loop Create(int nBeats, int maxSubBeats)
    {
        Loop l = new Loop();
        l.Beats = new Beat[nBeats];
        
        
        for (int i = 0; i < nBeats; i++)
        {
            int sbb = RandomSubBeats(maxSubBeats);
            l.Beats[i] = new Beat(sbb);
            if (sbb > 1)
            {
                maxSubBeats--;
            }
        }

        return l;
    }
}
