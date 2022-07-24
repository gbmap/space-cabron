using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;
using UnityEngine;

public class ImproviserTests 
{
    Melody melody;
    Improviser improviser;
    [SetUp]
    public void Init()
    {
        melody = new Melody("c4/4");
        improviser = new Improviser();
    }


    [Test]
    public static void ThrowsExceptionOnNullImprovisation()
    {
        Assert.Throws<ArgumentNullException>(() => new Improviser().AddImprovisation(null));
    }

    [Test]
    public void EmptyImproviserReturnsSameNote()
    {
        Note[] notes = improviser.Improvise(melody, 0, melody.GetNote(0), 0);
        Assert.IsTrue(notes.Length == 1);
        Assert.AreEqual(melody.GetNote(0), notes[0]);
    }

    [Test]
    public void AppliesImprovisationToOneNote()
    {
        improviser.AddImprovisation(
            new DuplicateNoteImprovisation(new EveryNStrategy(1), new EveryNStrategy(1))
        );
        Note[] notes = improviser.Improvise(melody, 0, melody.GetNote(0), 0);
        Assert.IsTrue(notes.Length == 2);
        Assert.AreEqual(melody.GetNote(0), notes[0]);
        Assert.AreEqual(melody.GetNote(0), notes[1]);
    }

    [Test]
    public void RemovingImprovisationGeneratesPreviousNote()
    {
        Improvisation improvisation = new DuplicateNoteImprovisation(
            new EveryNStrategy(1), new EveryNStrategy(1)
        );
        improviser.AddImprovisation(improvisation);
        improviser.Improvise(melody, 0, melody.GetNote(0), 0);
        improviser.RemoveImprovisation(improvisation);
        Note[] notes = improviser.Improvise(melody, 0, melody.GetNote(0), 0);
        Assert.AreEqual(notes.Length,1);
        Assert.AreEqual(melody.GetNote(0), notes[0]);
    }
}
