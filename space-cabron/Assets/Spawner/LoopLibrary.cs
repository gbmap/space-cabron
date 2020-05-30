using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Beat
{
    public int[][] subBeats;
    public int Index;

    public int CurrentAudio { get { return subBeats[Index][0]; } }
    public int[] GetCurrentBeatNotes()
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

    public int GetInstrumentAudio()
    {
        return CurrentBeat.CurrentAudio;
    }

    public int[] GetInstrumentAudios()
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

    public int[] Weights = new int[] { 2, 2, 2, 1, 1 };
    public int MaxSubBeats = 1;
    public int MaxInstrumentsInBeat = 1;

    public LoopCreator(int[] weights, int maxSubBeats, int maxInstrumentsInBeat)
    {
        Weights = weights;
        MaxSubBeats = maxSubBeats;
        MaxInstrumentsInBeat = maxInstrumentsInBeat;
    }

    public int RandomSubBeats(int MaxSubBeats)
    {
        return 1+Mathf.RoundToInt(Mathf.Round(Mathf.Pow(UnityEngine.Random.value, 1f)*MaxSubBeats) % MaxSubBeats);
    }

    private Loop Create(int nBeats, ShuffleBag<int> beats)
    {
        return Create(nBeats, MaxSubBeats, MaxInstrumentsInBeat, beats);
    }

    public Loop Create(int nBeats, int maxSubBeats, int maxInstrumentsInBeat, int[] weights)
    {
        return Create(nBeats, maxSubBeats, maxInstrumentsInBeat, CreateBag(weights));
    }

    public Loop Create(int nBeats, int maxSubBeats, int maxInstrumentsInBeat, ShuffleBag<int> weights)
    {
        Loop l = new Loop();
        l.Beats = new Beat[nBeats];

        for (int i = 0; i < nBeats; i++)
        {
            Beat b = new Beat();

            int nSubBeats = RandomSubBeats(maxSubBeats);
            int nInstruments = UnityEngine.Random.Range(1, maxInstrumentsInBeat);

            b.subBeats = new int[nSubBeats][];
            for (int iSubBeat = 0; iSubBeat < nSubBeats; iSubBeat++)
            {
                b.subBeats[iSubBeat] = GenerateBeatInstruments(nInstruments, weights);
            }

            l.Beats[i] = b;
        }

        return l;
    }

    public Loop Create(int nBeats, int[] weights)
    {
        ShuffleBag<int> beats = CreateBag(weights);
        return Create(nBeats, beats);
    }

    public Loop Create(int nBeats)
    {
        return Create(nBeats, Weights);
    }

    private static ShuffleBag<int> CreateBag(int[] weights)
    {
        ShuffleBag<int> beats = new ShuffleBag<int>();

        for (int i = 0; i < weights.Length; i++)
        {
            beats.Add(i, weights[i]);
        }

        return beats;
    }

    private static int[] GenerateBeatInstruments(int nInstruments, ShuffleBag<int> beats, int none=-1)
    {
        int[] insts = new int[nInstruments];
        for (int i = 0; i < nInstruments; i++)
        {
            insts[i] = beats.Next();
            if (insts[i] == none)
            {
                for (int j = i; j < nInstruments; j++)
                {
                    insts[j] = none;
                }
                break;
            }
        }
        return insts;
    }
}
