using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;

public class NoteTests 
{
    [Test]
    public static void NoteEqualsWorkForEqualityOperator()
    {
        Note n1 = new Note(ENote.A, 4, 4);
        Note n2 = new Note(ENote.A, 4, 4);
        Assert.IsTrue(n1 == n2);
    }

    [Test]
    public static void NoteEqualsNoteWorksForReference()
    {
        Note n1 = new Note(ENote.A, 4, 4);
        Note n2 = new Note(ENote.A, 4, 4);
        Assert.IsTrue(n1.Equals(n2));
    }


    [Test]
    public static void NoteOffset()
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

    [TestCase(4, 1f, ExpectedResult=1f)]
    [TestCase(4, 0.5f, ExpectedResult=2f)]
    [TestCase(8, 1f, ExpectedResult=0.5f)]
    [TestCase(8, 2f, ExpectedResult=0.25f)]
    [TestCase(16, 1f, ExpectedResult=0.25f)]
    [TestCase(32, 1f, ExpectedResult=0.125f)]
    [TestCase(64, 1f, ExpectedResult=0.0625f)]
    public static float NoteDuration(int interval, float beatsPerSecond)
    {
        return new Note(ENote.A, interval, 3).GetDuration(beatsPerSecond);
    }


    [TestCase(60, ExpectedResult=1f)]
    [TestCase(120, ExpectedResult=2f)]
    [TestCase(30, ExpectedResult=0.5f)]
    [TestCase(15, ExpectedResult=0.25f)]
    [TestCase(45, ExpectedResult=0.75f)]
    public static float BPMToBPS(int bpm)
    {
        return Turntable.BPMToBPS(bpm);
    }

}
