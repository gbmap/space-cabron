using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSequencer : Instrument<ENote>
{
    public System.Action<ENote[]> OnNotePlayed;

    public override int NoNote => (int)ENote.None;

    protected override ENote FromInt(int v)
    {
        return (ENote)v;
    }

    protected override void OnNoteCallback(ENote[] note)
    {
        OnNotePlayed?.Invoke(note);
    }

    protected override int ToInt(ENote v)
    {
        return (int)v;
    }
}
