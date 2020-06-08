using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSynth : Synth
{
    public ENote TargetNote;
    public ENote OverrideNote;

    protected override void OnNoteCallback(ENote[] notes)
    {
        var note = notes[0];
        if (note != TargetNote)
        {
            note = ENote.None;
        }
        else
        {
            note = OverrideNote;
        }

        base.OnNoteCallback(notes);

        //OverrideNote = (ENote)noteSequencer.NoteBag.Next();
    }
}
