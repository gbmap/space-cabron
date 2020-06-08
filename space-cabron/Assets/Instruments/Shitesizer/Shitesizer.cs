using System.Linq;
using UnityEngine;

public class Shitesizer : MonoBehaviour
{
    public float hz = 440;
    public float gain = 0.1f;

    const float samplingFreq = 44000;

    float phase = 0f;

    public float[] Buffer
    {
        get; private set;
    }

    float t = 0f;
    private void Update()
    {
        t = Time.time;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i+=channels)
        {
            float w2 = Mathf.PI * 2f / 1f;
            float w = (hz + Mathf.Sin(phase+t*w2) * 200f ) * Mathf.PI * 2f;
            float f = w * (1f / samplingFreq);
            phase += f;

            data[i] += Mathf.Sin(phase) * gain;

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            if (phase > Mathf.PI *2f)
            {
                phase = 0f;
            }
        }

        Buffer = data.ToArray();
    }
}
