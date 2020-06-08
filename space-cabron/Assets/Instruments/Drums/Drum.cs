using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Drum : Instrument<EInstrumentAudio>
{
    public AudioSource Source;
    public InstrumentAudioSample Instrument;

    public override int NoNote => (int)EInstrumentAudio.None;

    protected override void OnNoteCallback(EInstrumentAudio[] s)
    {
        foreach (EInstrumentAudio audio in s)
        {
            if (audio == EInstrumentAudio.None) continue;
            Source.PlayOneShot(Instrument.GetAudio(audio));
        }
        
    }

    protected override EInstrumentAudio FromInt(int v)
    {
        return (EInstrumentAudio)v;
    }

    protected override int ToInt(EInstrumentAudio v)
    {
        return (int)v;
    }
}
