using UnityEngine;

[System.Serializable]
public class SynthInstrument : ScriptableObject
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
    }

    public EWave Wave = EWave.Sine;
    [Range(0f, 1f)]
    public float Amplitude = 1f;
    public float FrequencyFactor = 1f;

    public float Sample(float f)
    {
        // TODO: implement
        switch (Wave)
        {
            default:
            case EWave.Tri: return Mathf.PingPong(f, 1f);
            case EWave.Sine: return Mathf.Sin(f);
            case EWave.Square: return Mathf.Sin(f) > 0.0f ? 1f : -1f;
            case EWave.Noise: return Random.value;
        }
    }
}

