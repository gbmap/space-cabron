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
        BreakNoteModifier breakNote = new BreakNoteModifier();
        
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
        ShiftNoteModifier shiftNote = new ShiftNoteModifier();
        shiftNote.Step = 1;
        Melody m = new Melody("c4/8");
        Melody m2 = shiftNote.Apply(m);
        Assert.AreEqual("c#4/8", m2.Notation);

        shiftNote.Step = 2;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("d4/8", m2.Notation);

        shiftNote.Step = 3;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("d#4/8", m2.Notation);

        shiftNote.Step = -1;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("b4/8", m2.Notation);

        shiftNote.Step = -12;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("c3/8", m2.Notation);

        shiftNote.Step = 12;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("c5/8", m2.Notation);

        shiftNote.Step = 13;
        m2 = shiftNote.Apply(m);
        Assert.AreEqual("c#5/8", m2.Notation);
    }
}
