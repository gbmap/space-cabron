using System;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;

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

    private Melody ImproviseOnMelody(Improvisation improvisation, Melody m) {
        improviser.AddImprovisation(improvisation);
        List<Note> lnotes = new List<Note>();
        for (int i = 0; i < m.Length; i++)
        {
            Note[] notes = improviser.Improvise(m, 0, m.GetNote(i), i);
            lnotes.AddRange(notes);
        }

        Melody m2 = new Melody(lnotes.ToArray());
        return m2;
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

    [TestCase("", 1, 1, ExpectedResult=null)]
    [TestCase("c4/4", -1, 1, ExpectedResult="c4/4")]
    [TestCase("c4/4", 1, 2, ExpectedResult="c4/4")]
    [TestCase("d4/4;c3/8", 1, 2, ExpectedResult="d4/4;c3/8;c3/8")]
    [TestCase("f3/4;g3/4;d3/4", 1, 2, ExpectedResult="f3/4;g3/4;g3/4;d3/4")]
    [TestCase("c4/4;c4/4;c4/4;c4/4", 1, 2, ExpectedResult="c4/4;c4/4;c4/4;c4/4;c4/4;c4/4")]
    [TestCase("c4/4;c4/4;c4/4;c4/4", 1, 2, ExpectedResult="c4/4;c4/4;c4/4;c4/4;c4/4;c4/4")]
    [TestCase("c4/4;d4/4;e4/4", 1, 1, ExpectedResult="c4/4;c4/4;d4/4;d4/4;e4/4;e4/4")]
    [TestCase("c4/16;f#4/16;a#4/16;c4/16;d3/8;e3/8", 2, 3, ExpectedResult="c4/16;f#4/16;a#4/16;a#4/16;a#4/16;c4/16;d3/8;e3/8;e3/8;e3/8")]
    public string DuplicateNote(string melody, int timesToDuplicate, int everyNNote)
    {
        Melody m = new Melody(melody);
        return ImproviseOnMelody(
            new DuplicateNoteImprovisation(new EveryNStrategy(everyNNote), new EveryNStrategy(1), timesToDuplicate),
            m
        ).Notation;
    }

    [TestCase("", 1, 1, ExpectedResult=null)]
    [TestCase("c4/4", -1, 1, ExpectedResult="c4/4")]
    [TestCase("c4/4", 1, 2, ExpectedResult="c4/4")]
    [TestCase("d4/4;c3/8", 1, 2, ExpectedResult="d4/4;c3/16;c3/16")]
    [TestCase("f3/4;g3/4;d3/4", 1, 2, ExpectedResult="f3/4;g3/8;g3/8;d3/4")]
    [TestCase("c4/4;c4/4;c4/4;c4/4", 1, 2, ExpectedResult="c4/4;c4/8;c4/8;c4/4;c4/8;c4/8")]
    [TestCase("c4/4;c4/4;c4/4;c4/4", 1, 2, ExpectedResult="c4/4;c4/8;c4/8;c4/4;c4/8;c4/8")]
    [TestCase("c4/4;d4/4;e4/4", 1, 1, ExpectedResult="c4/8;c4/8;d4/8;d4/8;e4/8;e4/8")]
    [TestCase("c4/16;f#4/16;a#4/16;c4/16;d3/8;e3/8", 2, 3, ExpectedResult="c4/16;f#4/16;a#4/48;a#4/48;a#4/48;c4/16;d3/8;e3/24;e3/24;e3/24")]
    public string BreakNote(string melody, int timesToDuplicate, int everyNNote)
    {
        Melody m = new Melody(melody);
        return ImproviseOnMelody(
            new BreakNoteImprovisation(new EveryNStrategy(everyNNote), new SelectAllStrategy(), timesToDuplicate),
            m
        ).Notation;
    }

    [TestCase("", 1, 1, ExpectedResult=null)]
    [TestCase("c4/4", 1, 1, ExpectedResult="c#4/4")]
    [TestCase("c4/4;c4/4", 1, 1, ExpectedResult="c#4/4;c#4/4")]
    [TestCase("c4/4;c4/4", 1, 2, ExpectedResult="c4/4;c#4/4")]
    [TestCase("c4/4;c4/4;c4/4", 2, 3, ExpectedResult="c4/4;c4/4;d4/4")]
    public string TransposeNote(string melody, int steps, int everyNNote)
    {
        Melody m = new Melody(melody);
        return ImproviseOnMelody(
            new TransposeNoteImprovisation(new EveryNStrategy(everyNNote), new EveryNStrategy(1), steps),
            m
        ).Notation;
    }

    [TestCase("", 1, 1, ExpectedResult=null)]
    [TestCase("c4/4", 1, 1, ExpectedResult="c4/16;c#4/16;c4/8")]
    [TestCase("c4/4", 1, 2, ExpectedResult="c4/16;d4/16;c4/8")]
    [TestCase("c4/4", 1, -1, ExpectedResult="c4/16;b3/16;c4/8")]
    [TestCase("c4/4", 1, -2, ExpectedResult="c4/16;a#3/16;c4/8")]
    [TestCase("c4/4", 1, 5, ExpectedResult="c4/16;f4/16;c4/8")]
    public string UpperMordent(string melody, int everyNNote, int steps)
    {
        return ImproviseOnMelody(
            new MordentImprovisation(new EveryNStrategy(everyNNote), new EveryNStrategy(1), steps),
            new Melody(melody)
        ).Notation;
    }

    [TestCase("c4/4", 1, 1, 1, ExpectedResult="c#4/8;c4/8")]
    [TestCase("c4/4", 1, 1, 2, ExpectedResult="c#4/16;c4/16;c#4/16;c4/16")]
    [TestCase("c4/4", 1, 1, 3, ExpectedResult="c#4/32;c4/32;c#4/32;c4/32;c#4/32;c4/32;c#4/32;c4/32")]
    public string Tremolo(string melody, int everyNNote, int steps, int beams)
    {
        return ImproviseOnMelody(
            new TremoloImprovisation(new EveryNStrategy(everyNNote), new EveryNStrategy(1), beams, steps),
            new Melody(melody)
        ).Notation;
    }

}
