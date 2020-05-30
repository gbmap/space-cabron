using UnityEngine;

public enum EInstrumentAudio
{
    Kick,
    Snare,
    FloorTom,
    HiHat,
    None,
}

[System.Serializable]
public class InstrumentAudioSample : ScriptableObject
{
    public AudioClip[] Instruments;

    public AudioClip GetAudio(EInstrumentAudio instrument)
    {
        return Instruments[(int)instrument];
    }
}