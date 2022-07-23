using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;
using UnityEngine;

public class MelodyModifierTests 
{
    [Test]
    public static void Test_BreakNoteModifier()
    {
        BreakNoteModifier breakNote = new BreakNoteModifier(2);
        
        Melody m = new Melody("c4/8");
        Melody m2 = breakNote.Apply(m);
        Assert.AreEqual("c4/16;c4/16", m2.Notation);
        breakNote.NumberOfNotes = 3;
        Melody m3 = breakNote.Apply(m);
        Assert.AreEqual("c4/24;c4/24;c4/24", m3.Notation);
    }

    [Test]
    public static void Test_ShiftNoteModifier()
    {
        ShiftNoteModifier shiftNote = new ShiftNoteModifier(1);
        Melody m = new Melody("c4/8");
        Melody m2 = shiftNote.Apply(m);
        Assert.AreEqual("c#4/8", m2.Notation);

        shiftNote.Steps = 2;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("d4/8", m2.Notation);

        shiftNote.Steps = 3;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("d#4/8", m2.Notation);

        shiftNote.Steps = -1;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("b3/8", m2.Notation);

        shiftNote.Steps = -12;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("c3/8", m2.Notation);

        shiftNote.Steps = 12;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("c5/8", m2.Notation);

        shiftNote.Steps = 13;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("c#5/8", m2.Notation);
    }

    [TestCase("c4/4", 1, 0, ExpectedResult="c#4/4")]
    [TestCase("c4/4", 1, -1, ExpectedResult="c#4/4")]
    [TestCase("c4/4", 1, -100, ExpectedResult="c#4/4")]
    [TestCase("c4/4", 1, 2, ExpectedResult="c4/4;c#4/4")]
    [TestCase("c4/4", 12, 2, ExpectedResult="c4/4;c5/4")]
    [TestCase("c4/4", 12, 3, ExpectedResult="c4/4;c4/4;c5/4")]
    [TestCase("g4/8;a4/8", 5, 4, ExpectedResult="g4/8;a4/8;g4/8;a4/8;g4/8;a4/8;c5/8;d5/8")]
    public static string Test_TransposeMelodyModifier(string notation, int steps, int everyN)
    {
        TransposeMelodyModifier modifier = new TransposeMelodyModifier(everyN, steps);
        Melody m = new Melody(notation);
        Melody result = modifier.Apply(m);
        return result.Notation;
    }
}
