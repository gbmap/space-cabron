using UnityEngine;

[System.Serializable]
public class SynthInstrument
{
    public Oscillator[] oscs;

    public float Sample(float f)
    {
        float amp = 0f;
        for (int i = 0; i < oscs.Length; i++)
        {
            var o = oscs[i];
            amp += o.Sample(f * o.FrequencyFactor) * o.Amplitude;
        }

        return amp;
    }

    public float Sample(float T, float hz)
    {
        float amp = 0f;
        for (int i = 0; i < oscs.Length; i++)
        {
            var o = oscs[i];

            float x = T * o.FrequencyFactor;

            if (o.Modulators != null)
            {
                for (int j = 0; j < o.Modulators.Length; j++)
                {
                    x += o.Modulators[j].Modulate(hz, T);
                }
            }
            float y = o.Sample(x) * o.Amplitude;

            y *= o.Amplitude;
            amp += y;
        }

        return amp;
    }
}

[System.Serializable]
public class LFModulator
{
    [Range(0f, 5f)]
    public float Amplitude = 0.01f;
    public float Frequency = 0.01f;

    public float Modulate(float hz, float t)
    {
        return Amplitude * hz * Mathf.Sin(Frequency * 2f * Mathf.PI * t);
    }
}

[System.Serializable]
public class Oscillator
{
    public enum EWave
    {
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
    [Range(0f, 1f)]
    public float Amplitude = 1f;
    public float FrequencyFactor = 1f;

    public LFModulator[] Modulators;
    public EnvelopeASDR envelope = new EnvelopeASDR();

    static System.Random rand = new System.Random();

    public float Sample(float f)
    {
        switch (Wave)
        {
            default:
            case EWave.Tri: return Mathf.PingPong(f, 1f);
            case EWave.Sine: return Mathf.Sin(f);
            case EWave.Square: return Mathf.Sin(f) > 0.0f ? 1f : -1f;
            case EWave.Noise: return ((float)rand.Next())/rand.Next();
            case EWave.Saw: return ((f % 1) - 0.5f) * 2f;
            // sin(x) - 1.0 * floor(sin(x)/1.0)
            case EWave.Planalto: return (float)(Mathf.Sin(f) - 1.0 * Mathf.Floor(Mathf.Sin(f) / 1.0f));

            // sin(x *(x%1))
            case EWave.WetVoices: return Mathf.Sin(f * (f % 1));
            case EWave.BigNoseFatBelly:
                {
                    // sin(x % 1 * (x % 1))
                    float x = Mathf.Sin( (f % 1) * (f % 1) );
                    // atan2(sin(x), -x) / PI + sin(x * 10) * 0.2
                    return Mathf.Atan2(Mathf.Sin(x), -x) / Mathf.PI + Mathf.Sin(x * 10) * 0.2f;
                }
            case EWave.Test: return Mathf.Pow(Mathf.Sin(f * (f % 1)), 3.0f);
            
        }
    }   
}

