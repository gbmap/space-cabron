using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Beat
{
    public EInstrumentAudio[][] subBeats;
    public int Index;

    public EInstrumentAudio CurrentAudio { get { return subBeats[Index][0]; } }
    public EInstrumentAudio[] GetCurrentBeatNotes()
    {
        return subBeats[Index];
    }

    public bool Advance()
    {
        Index++;
        if (Index == subBeats.Length)
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

    public EInstrumentAudio[] GetInstrumentAudios()
    {
        return CurrentBeat.GetCurrentBeatNotes();
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
        return 1+Mathf.RoundToInt(Mathf.Round(Mathf.Pow(UnityEngine.Random.value, 1f)*MaxSubBeats) % MaxSubBeats);
    }

    public static int MaxInstrumentsInBeat = 1;

    private static Loop Create(int nBeats, ShuffleBag<EInstrumentAudio> beats)
    {
        return Create(nBeats, MaxSubBeats, MaxInstrumentsInBeat, beats);
    }

    public static Loop Create(int nBeats, int maxSubBeats, int maxInstrumentsInBeat, int[] weights)
    {
        return Create(nBeats, maxSubBeats, maxInstrumentsInBeat, CreateBag(weights));
    }

    public static Loop Create(int nBeats, int maxSubBeats, int maxInstrumentsInBeat, ShuffleBag<EInstrumentAudio> weights)
    {
        Loop l = new Loop();
        l.Beats = new Beat[nBeats];

        for (int i = 0; i < nBeats; i++)
        {
            Beat b = new Beat();

            int nSubBeats = RandomSubBeats(maxSubBeats);
            int nInstruments = UnityEngine.Random.Range(1, maxInstrumentsInBeat);

            b.subBeats = new EInstrumentAudio[nSubBeats][];
            for (int iSubBeat = 0; iSubBeat < nSubBeats; iSubBeat++)
            {
                b.subBeats[iSubBeat] = GenerateBeatInstruments(nInstruments, weights);
            }

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

    private static ShuffleBag<EInstrumentAudio> CreateBag(int[] weights)
    {
        ShuffleBag<EInstrumentAudio> beats = new ShuffleBag<EInstrumentAudio>();

        for (int i = 0; i < weights.Length; i++)
        {
            beats.Add((EInstrumentAudio)i, weights[i]);
        }

        return beats;
    }

    private static EInstrumentAudio[] GenerateBeatInstruments(int nInstruments, ShuffleBag<EInstrumentAudio> beats)
    {
        EInstrumentAudio[] insts = new EInstrumentAudio[nInstruments];
        for (int i = 0; i < nInstruments; i++)
        {
            insts[i] = beats.Next();
            if (insts[i] == EInstrumentAudio.None)
            {
                for (int j = i; j < nInstruments; j++)
                {
                    insts[j] = EInstrumentAudio.None;
                }
                break;
            }
        }
        return insts;
    }
}

public class LoopLibrary : ScriptableObject
{
    public List<Loop> Loops;
    public Loop[] Turnarounds;
}

