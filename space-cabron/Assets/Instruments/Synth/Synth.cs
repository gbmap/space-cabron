using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[System.Serializable]
public class EnvelopeASDR
{
    [Range(0f, 5f)]
    public double attackTime = 0.25f;

    [Range(0f, 1f)]
    public double attackAmplitude = 1f;

    [Range(0f, 5f)]
    public double decayTime = 0.25f;

    [Range(0f, 1f)]
    public double sustainAmplitude = 0.8f;

    [Range(0f, 5f)]
    public double releaseTime = 0.3f;

    bool pressed;
    float tPress;
    float tRelease;

    public double GetAmplitude(float p)
    {
        double a = 0.0f;
        double t = p - tPress;

        if (pressed)
        {
            if (t <= attackTime)
            {
                a = (t / attackTime) * attackAmplitude;
            }
            else if (t > attackTime && t <= attackTime+decayTime)
            {
                a = ((t - attackTime) / decayTime) * (sustainAmplitude - attackAmplitude) + attackAmplitude;
            }
            else if (t > attackTime + decayTime)
            {
                a = sustainAmplitude;
            }
        }
        else
        {
            a = ((p - tRelease) / releaseTime) * (0.0f - sustainAmplitude) + sustainAmplitude;
        }

        if (a <= 0.0001)
            a = 0f;

        return a;
    }

    public void KeyOn()
    {
        tPress = Time.time;
        pressed = true;
    }

    public void KeyOff()
    {
        tRelease = Time.time;
        pressed = false;
    }
}

public enum ENote
{
    A,
    Asharp,
    B,
    C,
    Csharp,
    D,
    Dsharp,
    E,
    F,
    Fsharp,
    G,
    Gsharp,
    None
}

public enum ENoteTime
{
    OnBeat,
    OnBeatNoSubBeat,
    OnBar
}

[System.Serializable]
public class NoteChance
{
    public ENote note;
    public int Weight;
}

public class Synth : BeatMakerBehaviour
{
    public AudioSource audioSource;
    public int BPM = 100;
    public int MaxSubBeats = 1;
    public int MaxInstrumentsPerBeat = 1;
    public int NBeats = 8;

    public int Octave = 3;

    public ENoteTime noteTime;
    public bool HoldNote;
    public List<NoteChance> NoteWeights = new List<NoteChance>();

    int noteIndex;
    
    [Range(0f, 1f)]
    public float Gain;

    public SynthInstrument Instrument;
    public EnvelopeASDR Envelope;

    public double Frequency = 440.0;

    float[] prevData;
    double increment;
    double phase;
    double samplingFrequency = 44000.0;
    bool on;
    float t;

    private double a;

    private void Awake()
    {
        BeatMaker = new BeatMaker(NoteWeights.Select(n => n.Weight).ToArray(), 
            MaxSubBeats, 
            MaxInstrumentsPerBeat, 
            NBeats, 
            BPM
        );
        BeatMaker.Run();
        BeatMaker.OnBar += OnBar;
        BeatMaker.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        BeatMaker.OnBar -= OnBar;
        BeatMaker.OnBeat -= OnBeat;
    }

    private void OnBar(int bar)
    {
        
    }

    private void OnBeat(int[] notes)
    {
        if (noteTime != ENoteTime.OnBeat) return;

        foreach (int n in notes)
        {
            if (n == (int)ENote.None) return;

            Envelope.KeyOff();
            Envelope.KeyOn();
            ENote note = (ENote)n;
            Frequency = NoteToFrequency(note, Octave);
            if (!HoldNote) Envelope.KeyOff();
        }
    }

    private void Update()
    {
        BeatMaker.Update(true);
        t = Time.time;
        a = (float)Envelope.GetAmplitude(t);
        audioSource.volume =(float)a;
    }

    double f;
    private void OnAudioFilterRead(float[] data, int channels)
    {
        double w = Frequency * 2.0 * Mathf.PI;
        f = w / samplingFrequency;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += f;
            float p = (float)phase;
            data[i] += Gain * Instrument.Sample(p, (float)Frequency);

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

        }

        prevData = data;
    }

    public static string[] Notes = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "None" };
    public float NoteToFrequency(string note)
    {
        int octave = (note.Length == 3 ? note[2] : note[1]) - '0';

        int keyNumber = Array.IndexOf(Notes, note.Substring(0, note.Length - 1));
        if (keyNumber < 3)
        {
            keyNumber = keyNumber + ((octave - 1) * 12) + 1;
        }
        else
        {
            keyNumber = keyNumber + ((octave - 1) * 12) + 1;
        }

        return 440f * Mathf.Pow(2f, ((float)keyNumber - 49f) / 12f);
    }

    public float NoteToFrequency(ENote note, int octave)
    {
        string n = Notes[(int)note] + octave.ToString();
        return NoteToFrequency(n);
    }

    public void ChangePattern(bool updateValues = false)
    {
        if (updateValues)
        {
            BeatMaker.BPM = BPM;
            BeatMaker.LoopCreator.MaxSubBeats = MaxSubBeats;
            BeatMaker.LoopCreator.MaxInstrumentsInBeat = MaxInstrumentsPerBeat;
            BeatMaker.LoopCreator.NBeats = NBeats;
            BeatMaker.LoopCreator.Weights = NoteWeights.Select(n => n.Weight).ToArray();
        }
        BeatMaker.RefreshLoop();
    }

    public static List<NoteChance> GenerateNoteWeights(int[] weights)
    {
        List<NoteChance> notes = new List<NoteChance>();

        var values = Enum.GetValues(typeof(ENote));
        int i = 0;
        foreach (var v in values)
        {
            notes.Add(new NoteChance
            {
                note = (ENote)v,
                Weight = weights[i]
            });
            i++;
        }

        return notes;
    }

    private void OnGUI()
    {
        BeatMaker.OnGUI();

    }

}
