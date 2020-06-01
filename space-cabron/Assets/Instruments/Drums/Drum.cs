using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Drum : Instrument<EInstrumentAudio>
{
    public AudioSource Source;
    public InstrumentAudioSample Instrument;

    public override int NoNote => (int)EInstrumentAudio.None;

    protected override void OnNoteCallback(EInstrumentAudio s)
    {
        if (s == EInstrumentAudio.None) return;
        Source.PlayOneShot(Instrument.GetAudio(s));
    }

    protected override EInstrumentAudio FromInt(int v)
    {
        return (EInstrumentAudio)v;
    }
}
