using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pxMelodyPlayer : Z.ZBaseMelodyPlayer
{
    public pxStrax px;

    public override void Play(ENote note, int octave)
    {
        int midi = (10 * octave) + (int)note;

        px.KeyOn(midi);
    }



}
