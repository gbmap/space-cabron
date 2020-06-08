using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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

    /*

    public double GetAmplitude(double p, double tPress, double tRelease, bool pressed, double A = 0.0)
    {
        double a = A;
        double t = p - tPress;

        if (pressed)
        {
            if (t <= attackTime)
            {
                //a = ((t / attackTime) * attackAmplitude);
                a = Synth.dLerp(a, attackAmplitude, (t / attackTime));
            }
            if (t > attackTime)
            {
                a = Synth.dLerp(a, sustainAmplitude, (t - attackTime) / decayTime);
                //a = ((t - attackTime) / decayTime) * (sustainAmplitude - attackAmplitude) + attackAmplitude;
            }
        }
        else
        {
            a = Synth.dLerp(a, 0.0, (p - tRelease) / releaseTime);
            //a = ((p - tRelease) / releaseTime) * (0.0 - sustainAmplitude) + sustainAmplitude;
        }

        return System.Math.Max(0.0, System.Math.Min(1.0, a));
    }

    */
    
    public double GetAmplitude(double t, double tPress, double tRelease, bool pressed, double A = 0.0)
    {
        double a = 0.0;
        double releaseAmplitude = 0.0;

        if (tPress > tRelease)
        {
            double lifetime = t - tPress;

            if (lifetime <= attackTime)
                a = (lifetime / attackTime) * attackAmplitude;

            if (lifetime > attackTime && lifetime <= (attackTime + decayTime))
                a = ((lifetime - attackTime) / decayTime) * (sustainAmplitude - attackAmplitude) + attackAmplitude;

            if (lifetime > (attackTime + decayTime))
                a = sustainAmplitude;
        }
        else
        {
            double lifetime = tRelease - tPress;

            if (lifetime <= attackTime)
                releaseAmplitude = (lifetime / attackTime) * attackAmplitude;

            if (lifetime > attackTime && lifetime <= (attackTime + decayTime))
                releaseAmplitude = ((lifetime - attackTime) / decayTime) * (sustainAmplitude - attackAmplitude) + attackAmplitude;

            if (lifetime > (attackTime + decayTime))
                releaseAmplitude = sustainAmplitude;

            a = ((t - tRelease) / releaseTime) * (0.0 - releaseAmplitude) + releaseAmplitude;
        }

        if (a <= 0.01)
            a = 0.0;

        return a;
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

public struct SynthNote
{
    public double Note;
    public bool Pressed;
    public double tPress;
    public double tRelease;
    public double Amplitude;
    public double Duration;
}

public class Synth : MonoBehaviour
{
    public AudioSource audioSource;
    public NoteSequencer NoteSequencer;

    public int Octave = 3;

    public ENoteTime noteTime;
    public bool HoldNote;

    int noteIndex;
    
    [Range(0f, 1f)]
    public double Gain;

    public SynthInstrument Instrument;
    public EnvelopeASDR Envelope;

    public double Frequency = 440.0;

    public float[] PrevData
    {
        get; private set;
    }

    double increment;
    double phase;
    public static double sampleRate = 48000.0;
    bool on;

    private double a;

    /////////////////////////
    /// NOTE MANAGEMENT
    /// 
    private List<SynthNote> notes = new List<SynthNote>();

    public void PlayKey(ENote note, int octave, double duration=0.01)
    {
        int i = notes.FindIndex(n => n.Note == NoteToFrequency(note, octave));
        var synthNote = new SynthNote
        {
            Note = NoteToFrequency(note, octave),
            tPress = AudioSettings.dspTime,
            tRelease = double.MinValue,
            Pressed = true,
            Amplitude = 0.0,
            Duration = duration
        };

        if (i != -1)
        {
            notes[i] = synthNote;
        }
        else
        {
            notes.Add(synthNote);
        }
    }

    public int PlayKey(ENote note, int octave, out SynthNote synthNote, double duration=0.01)
    {
        int i = notes.FindIndex(n => n.Note == NoteToFrequency(note, octave));
        synthNote = new SynthNote
        {
            Note = NoteToFrequency(note, octave),
            tPress = AudioSettings.dspTime,
            tRelease = double.MinValue,
            Pressed = true,
            Amplitude = 0.0,
            Duration = duration
        };

        if (i != -1)
        {
            notes[i] = synthNote;
            return i;
        }
        else
        {
            notes.Add(synthNote);
            return notes.Count - 1;
        }
    }

    public SynthNote StopKey(SynthNote sn)
    {
        sn.tRelease = AudioSettings.dspTime;
        sn.Pressed = false;
        return sn;
    }

    protected virtual void OnNoteCallback(ENote[] note)
    {
        for (int i = 0; i < notes.Count; i++)
        {
            SynthNote sn = notes[i];
            if (AudioSettings.dspTime > sn.tPress + sn.Duration && sn.Pressed)
            {
                notes[i] = StopKey(sn);
            }
        }

        if (note[0] == ENote.None) return;

        SynthNote n;
        int index = PlayKey(note[0], Octave, out n, 0.01);
        if (!HoldNote)
        {
            //notes[index] = StopKey(n);
        }

        //Frequency = NoteToFrequency(note[0], Octave); // todo: implementar multiplas notas
    }

    public static string[] Notes = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "None" };

    public double NoteToFrequency(string note)
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

        return 440.0 * Mathf.Pow(2f, ((float)keyNumber - 49f) / 12f);
    }

    public double NoteToFrequency(ENote note, int octave)
    {
        string n = Notes[(int)note] + octave.ToString();
        return NoteToFrequency(n);
    }

    ////////////////////////
    /// MONO BEHAVIOUR
    /// 
    private void OnEnable()
    {
        if (!NoteSequencer) return;
        NoteSequencer.OnNotePlayed += OnNoteCallback;
    }

    private void OnDisable()
    {
        if (!NoteSequencer) return;
        NoteSequencer.OnNotePlayed -= OnNoteCallback;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        var time = AudioSettings.dspTime;

        for (int i = 0; i < data.Length; i += channels)
        {
            double t = (time + i / sampleRate / channels);
            double hz = Frequency;

            double amp = 0.0;

            for (int ni = 0; ni < notes.Count; ni++)
            {
                SynthNote note = notes[ni];
                note.Amplitude = Envelope.GetAmplitude(t, note.tPress, note.tRelease, note.Pressed, note.Amplitude);
                Instrument.Update(note.Amplitude);

                amp += (float)(Instrument.Sample(note.Note, t) * note.Amplitude);

                notes[ni] = note;
                if (!note.Pressed && AudioSettings.dspTime >= note.tRelease + Envelope.releaseTime)
                {
                    notes.RemoveAt(ni--);
                }
            }

            data[i] += (float)amp;

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }

        PrevData = data.ToArray();
    }

    ///////////////
    /// UTILITY
    /// 

    public static double dLerp(double x, double y, double t)
    {
        t = System.Math.Max(0.0, System.Math.Min(t, 1.0));
        return x + ((y - x) * t);
    }
}
