using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatMakerBehaviour : MonoBehaviour
{
    public BeatMaker BeatMaker;
}

[System.Serializable]
public class SampleChance
{
    public EInstrumentAudio Sample;
    public int Weight;
}

public class Drum : BeatMakerBehaviour
{
    public AudioSource source;
    public InstrumentAudioSample instrument;

    public int BPM = 100;
    public int MaxSubBeats = 1;
    public int MaxInstrumentsPerBeat = 1;
    public int NBeats = 8;

    public List<SampleChance> SampleWeights = new List<SampleChance>();

    void Awake()
    {
        BeatMaker = new BeatMaker(SampleWeights.Select(w=>w.Weight).ToArray(), 
            MaxSubBeats,
            MaxInstrumentsPerBeat,
            NBeats,
            BPM);
        BeatMaker.Run();

        BeatMaker.OnBeat += OnBeat;
    }

    public void GenerateNewWeights()
    {
        SampleWeights = new List<SampleChance>();
        var values = System.Enum.GetValues(typeof(EInstrumentAudio));
        foreach (EInstrumentAudio v in values)
        {
            SampleWeights.Add(new SampleChance
            {
                Sample = v,
                Weight = 0
            });
        }
    }

    public void GenerateNewPattern(bool updateValues = false)
    {
        if (updateValues)
        {
            BeatMaker.BPM = BPM;
            BeatMaker.LoopCreator.MaxSubBeats = MaxSubBeats;
            BeatMaker.LoopCreator.MaxInstrumentsInBeat = MaxInstrumentsPerBeat;
            BeatMaker.LoopCreator.NBeats = NBeats;
            BeatMaker.LoopCreator.Weights = SampleWeights.Select(n => n.Weight).ToArray();
        }
        BeatMaker.RefreshLoop();
    }

    private void OnDisable()
    {
        BeatMaker.OnBeat -= OnBeat;    
    }

    // Update is called once per frame
    void Update()
    {
        BeatMaker.Update();
    }

    private void OnBeat(int[] notes)
    {
        EInstrumentAudio[] samples = notes.Select(n => (EInstrumentAudio)n).ToArray();

        foreach (var sample in samples)
        {
            if (sample == EInstrumentAudio.None) return;
            source.PlayOneShot(instrument.GetAudio(sample));
        }
    }

    private void OnGUI()
    {
        return;
        BeatMaker.OnGUI();
    }
}
