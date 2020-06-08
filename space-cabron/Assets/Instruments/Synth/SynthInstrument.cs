using UnityEngine;

[System.Serializable]
public class SynthInstrument
{
    public MainOscillator[] oscs = new MainOscillator[0];

    public double Sample(double hz, double t)
    {
        double amp = 0f;

        for (int i = 0; i < oscs.Length; i++)
        {
            var o = oscs[i];
            double y = o.Sample(hz, t);
            amp += y;
        }

        return amp;
    }

    public void Update(double envelopeA)
    {
        foreach (var osc in oscs)
        {
            if (osc.Amplitude.Controlled)
            {
                osc.Amplitude.UpdateValue(
                    osc.Amplitude.Driver == EControlledFloatDriver.Envelope ? envelopeA : 0.0
                    );
            }

            if (osc.FrequencyFactor.Controlled)
            {
                osc.FrequencyFactor.UpdateValue(
                    osc.Amplitude.Driver == EControlledFloatDriver.Envelope ? envelopeA : 0.0
                    );
            }
        }
    }
}

[System.Serializable]
public class LFModulator
{
    [Range(0f, 5f)]
    public float Amplitude = 0.01f;
    public float Frequency = 0.01f;

    public LFModulator()
    {

    }

    public LFModulator(float a, float f)
    {
        Amplitude = a;
        Frequency = f;
    }

    public float Modulate(float hz, float t)
    {
        return Amplitude * hz * Mathf.Sin(Frequency * 2f * Mathf.PI * t);
    }
}

[System.Serializable]
public class MainOscillator
{
    public Oscillator Oscillator = new Oscillator();

    public ControlledFloat FrequencyFactor = new ControlledFloat(1f);
    public ControlledFloat Amplitude = new ControlledFloat(1f);

    public double Sample(double hz, double t)
    {
        return Oscillator.Sample(hz*FrequencyFactor*t*System.Math.PI*2f) * Amplitude;
    }
}

[System.Serializable]
public class Oscillator
{
    public enum EWave
    {
        None,
        Sine,
        Tri,
        Square,
        Noise,
        Saw,
        Planalto,
        WetVoices,
        BigNoseFatBelly,
        Test
    }

    public EWave Wave = EWave.Sine;

    static System.Random rand = new System.Random();

    public double Sample(double t)
    {
        switch (Wave)
        {
            default:
            case EWave.None: return 0f;
            case EWave.Tri:
                {
                    double p = System.Math.PI;
                    return (System.Math.Abs((((t+p*1.5) % (p * 2.0)) - p) / p) - 0.5)*2.0;
                }
            case EWave.Sine: return System.Math.Sin(t);
            case EWave.Square: return System.Math.Sin(t) >= 0.0f ? 1f : -1f;
            case EWave.Noise: return ((float)rand.Next())/rand.Next();
            case EWave.Saw:
                {
                    double p = System.Math.PI;
                    return (( (t+p) % (p*2.0) ) - p) / p;
                }

            // (sin(x) - 1.0 * floor(sin(x))) * 2 - 1
            case EWave.Planalto: return (System.Math.Sin(t) - 1.0 * System.Math.Floor(System.Math.Sin(t)))*2.0-1.0;

            // sin( (x%(PI*2)) * (x%(PI*2)) )
            case EWave.WetVoices:
                {
                    double pi = System.Math.PI;
                    return System.Math.Sin((t % (pi * 2.0)) * (t % (pi * 2.0)));
                }
            case EWave.BigNoseFatBelly:
                {
                    //t = t % Mathf.PI * 2f;
                    t += Mathf.PI*1.5f;
                    // sin(x % 1 * (x % 1))
                    double pi = System.Math.PI;
                    double x = System.Math.Sin( (t/(pi*2.0) % 1.0) * (t/(pi*2.0) % 1.0) );
                    // atan2(sin(x), -x) / PI + sin(x * 10) * 0.2
                    x = ((System.Math.Atan2(System.Math.Sin(x), -x) / System.Math.PI) + (System.Math.Sin(x * 10) * 0.2f));
                    return (x-0.8)*4.0;
                }
            case EWave.Test: return System.Math.Pow(System.Math.Sin(t * (t % 1)), 3.0f);
            
        }
    }
}

