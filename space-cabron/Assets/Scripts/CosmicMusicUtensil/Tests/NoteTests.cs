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
        Assert.AreEqual(ENote.A, Note.OffsetNote(ENote.A, 0));
        Assert.AreEqual(ENote.Asharp, Note.OffsetNote(ENote.A, 1));
        Assert.AreEqual(ENote.B,      Note.OffsetNote(ENote.A, 2));
        Assert.AreEqual(ENote.C,      Note.OffsetNote(ENote.A, 3));
        Assert.AreEqual(ENote.Gsharp, Note.OffsetNote(ENote.A, -1));
        Assert.AreEqual(ENote.G,      Note.OffsetNote(ENote.A, -2));
        Assert.AreEqual(ENote.A,      Note.OffsetNote(ENote.A, 12));
        Assert.AreEqual(ENote.B,      Note.OffsetNote(ENote.C, -1));
    }

    [Test]
    public static void TestNoteTranspose()
    {
        Note n = new Note(ENote.C, 4, 3);
        n.Transpose(1);
        Assert.AreEqual(ENote.Csharp, n.Tone);
        Assert.AreEqual(3, n.Octave);

        n.Transpose(24);
        Assert.AreEqual(ENote.Csharp, n.Tone);
        Assert.AreEqual(5, n.Octave);
    }

    [Test]
    public static void Transpose12IncreasesOctave()
    {
        Note n = new Note(ENote.C, 4, 3);
        n.Transpose(12);
        Assert.AreEqual(ENote.C, n.Tone);
        Assert.AreEqual(4, n.Octave);
    }

    [Test]
    public static void Transpose12NegativeDecreasesOctave()
    {
        Note n = new Note(ENote.C, 4, 3);
        n.Transpose(-12);
        Assert.AreEqual(ENote.C, n.Tone);
        Assert.AreEqual(2, n.Octave);
        n.Transpose(-24);
        Assert.AreEqual(ENote.C, n.Tone);
        Assert.AreEqual(0, n.Octave);
        n.Transpose(-25);
        Assert.AreEqual(ENote.B, n.Tone);
        Assert.AreEqual(-2, n.Octave);
    }

    [Test]
    public static void NoteNegativeTransposeDecreasesOctave()
    {
        Note n = new Note(ENote.C, 4, 3);
        n.Transpose(-1);
        Assert.AreEqual(ENote.B, n.Tone);
        Assert.AreEqual(2, n.Octave);
    }

    [Test]
    public static void PositiveTransposeIncreasesOctave()
    {
        Note n = new Note(ENote.A, 4, 3);
        n.Transpose(3);
        Assert.AreEqual(ENote.C, n.Tone);
        Assert.AreEqual(4, n.Octave);
    }

}
