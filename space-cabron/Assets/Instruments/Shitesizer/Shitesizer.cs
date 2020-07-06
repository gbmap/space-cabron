using UnityEngine;

public class Shitesizer : MonoBehaviour
{
    public float sampleRate = 48000f; 

    float step;
    float t;

    void Awake()
    {
        SetNote(69f);
    }

    public void SetNote(float note)
    {
        float freq = 440.0f * Mathf.Pow(2.0f, 1.0f * (note - 69f) / 12.0f);
        step = freq / sampleRate;
    }

    public float Run()
    {
        t += step;
        return Mathf.Sin(t);
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i+=channels)
        {
            data[i] += Run() * 2.0f;

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }
    }
}
