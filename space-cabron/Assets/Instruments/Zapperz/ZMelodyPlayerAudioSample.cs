using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Z {
    public class ZMelodyPlayerAudioSample : ZBaseMelodyPlayer
    {
        public InstrumentAudioSample Instrument;

        public override void Play(ENote note, int octave)
        {
            var sample = Instrument.GetAudio(note);
            AudioSource.PlayClipAtPoint(sample, Vector3.zero);
        }
    }
}