using UnityEngine;

public enum EInstrumentAudio
{
    None = 0,
    Kick,
    Snare,
    FloorTom,
    HiHat,
    Spawn1,
    Spawn2,
    Spawn3,
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