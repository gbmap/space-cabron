using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Beat
{
    public EInstrumentAudio[] instrumentAudio;
    public int Index;

    public EInstrumentAudio CurrentAudio { get { return instrumentAudio[Index]; } }
    public EInstrumentAudio LastAudio { get { return instrumentAudio[Mathf.Max(0, Index - 1)]; } }

    public bool Advance()
    {
        Index++;
        if (Index == instrumentAudio.Length)
        {
            Index = 0;
            return true;
        }
        return false;
    }
}

public class Loop
{
    public Beat[] Beats;
    public int Index;

    public Beat CurrentBeat { get { return Beats[Index]; } }
    public Beat LastBeat { get { return Beats[Mathf.Max(0, Index - 1)]; } }

    public EInstrumentAudio GetInstrumentAudio()
    {
        return CurrentBeat.CurrentAudio;
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

    public static int[] Weights = new int[] { 2, 2, 2, 1, 1 };
    public static int MaxSubBeats = 1;
    public static int RandomSubBeats(int MaxSubBeats)
    {
        return 1+Mathf.RoundToInt(Mathf.Round(Mathf.Pow(Random.value, 1f)*MaxSubBeats) % MaxSubBeats);
    }

    private static ShuffleBag<EInstrumentAudio> CreateBag(int[] weights)
    {
        ShuffleBag<EInstrumentAudio> beats = new ShuffleBag<EInstrumentAudio>();

        for (int i = 0; i < weights.Length; i++)
        {
            beats.Add((EInstrumentAudio)i, weights[i]);
        }

        return beats;
    }

    private static Loop Create(int nBeats, ShuffleBag<EInstrumentAudio> beats)
    {
        Loop l = new Loop();
        l.Beats = new Beat[nBeats];

        for (int i = 0; i < nBeats; i++)
        {
            Beat b = new Beat();
            b.instrumentAudio = beats.Next(RandomSubBeats(MaxSubBeats));
            l.Beats[i] = b;
        }

        return l;
    }

    public static Loop Create(int nBeats, int[] weights)
    {
        ShuffleBag<EInstrumentAudio> beats = CreateBag(weights);
        return Create(nBeats, beats);
    }

    public static Loop Create(int nBeats)
    {
        return Create(nBeats, Weights);
    }
}

public class LoopLibrary : ScriptableObject
{
    public List<Loop> Loops;
    public Loop[] Turnarounds;
}

