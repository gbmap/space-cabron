using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;

public class NoteTests 
{
    [Test]
    public static void TestNoteOffset()
    {
        Assert.AreEqual(ENote.Asharp, Note.OffsetNote(ENote.A, 1));
        Assert.AreEqual(ENote.B,      Note.OffsetNote(ENote.A, 2));
        Assert.AreEqual(ENote.C,      Note.OffsetNote(ENote.A, 3));
        Assert.AreEqual(ENote.Gsharp, Note.OffsetNote(ENote.A, -1));
        Assert.AreEqual(ENote.G,      Note.OffsetNote(ENote.A, -2));
        Assert.AreEqual(ENote.A,      Note.OffsetNote(ENote.A, 12));
        Assert.AreEqual(ENote.B,      Note.OffsetNote(ENote.C, -1));
    }
}
