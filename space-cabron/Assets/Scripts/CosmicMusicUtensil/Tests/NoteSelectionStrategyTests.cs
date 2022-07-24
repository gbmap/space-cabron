using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;
using UnityEngine;

public class NoteSelectionStrategyTests 
{
    [TestCase(0, 1, ExpectedResult=true)]
    [TestCase(0, 2, ExpectedResult=true)]
    [TestCase(0, 3, ExpectedResult=true)]
    [TestCase(0, 4, ExpectedResult=true)]
    [TestCase(0, 5, ExpectedResult=true)]
    [TestCase(1, 1, ExpectedResult=true)]
    [TestCase(1, 2, ExpectedResult=true)]
    [TestCase(1, 3, ExpectedResult=true)]
    [TestCase(1, 4, ExpectedResult=true)]
    [TestCase(1, 5, ExpectedResult=true)]
    [TestCase(-1, 1, ExpectedResult=true)]
    [TestCase(-1, 2, ExpectedResult=true)]
    [TestCase(-1, 3, ExpectedResult=true)]
    [TestCase(-1, 4, ExpectedResult=true)]
    [TestCase(-1, 5, ExpectedResult=true)]
    [TestCase(2, 1, ExpectedResult=true)]
    [TestCase(2, 0, ExpectedResult=false)]
    [TestCase(3, 0, ExpectedResult=false)]
    [TestCase(3, 1, ExpectedResult=false)]
    [TestCase(3, 2, ExpectedResult=true)]
    [TestCase(3, 4, ExpectedResult=false)]
    public bool EveryNBarSpecificCases(int everyNBar, int barIndex)
    {
        NoteSelectionStrategy strategy = new EveryNBarStrategy(everyNBar);
        return strategy.ShouldSelectNote(null, barIndex, Random.Range(0, 12));
    }


    [TestCase(2, 0, 1)]
    [TestCase(2, 2, 2)]
    [TestCase(0, 0, 1)]
    [Test]
    public void RandomEveryNBar(
        [Random(0, 32, 10)] int everyNBar, 
        [Random(0, 100, 10)] int noteIndex, 
        [Random(0, 100, 30)] int barIndex
    ) {
        NoteSelectionStrategy strategy = new EveryNBarStrategy(everyNBar);
        Assert.AreEqual(strategy.ShouldSelectNote(null, barIndex, noteIndex), (barIndex+1) % Mathf.Max(1, everyNBar) == 0);
    }

    [TestCase(0, 1, ExpectedResult=true)]
    [TestCase(0, 2, ExpectedResult=true)]
    [TestCase(0, 3, ExpectedResult=true)]
    [TestCase(0, 4, ExpectedResult=true)]
    [TestCase(0, 5, ExpectedResult=true)]
    [TestCase(1, 1, ExpectedResult=true)]
    [TestCase(1, 2, ExpectedResult=true)]
    [TestCase(1, 3, ExpectedResult=true)]
    [TestCase(1, 4, ExpectedResult=true)]
    [TestCase(1, 5, ExpectedResult=true)]
    [TestCase(-1, 1, ExpectedResult=true)]
    [TestCase(-1, 2, ExpectedResult=true)]
    [TestCase(-1, 3, ExpectedResult=true)]
    [TestCase(-1, 4, ExpectedResult=true)]
    [TestCase(-1, 5, ExpectedResult=true)]
    [TestCase(2, 1, ExpectedResult=true)]
    [TestCase(2, 0, ExpectedResult=false)]
    [TestCase(3, 0, ExpectedResult=false)]
    [TestCase(3, 1, ExpectedResult=false)]
    [TestCase(3, 2, ExpectedResult=true)]
    [TestCase(3, 4, ExpectedResult=false)]
    public bool EveryNNoteSpecificCases(int everyNNote, int noteIndex)
    {
        NoteSelectionStrategy strategy = new EveryNNoteStrategy(everyNNote);
        return strategy.ShouldSelectNote(null, Random.Range(0, 12), noteIndex);
    }


    [Test]
    public void RandomEveryNNote(
        [Random(0, 32, 10)] int everyNNote, 
        [Random(0, 100, 10)] int noteIndex, 
        [Random(0, 100, 30)] int barIndex
    ) {
        NoteSelectionStrategy strategy = new EveryNNoteStrategy(everyNNote);
        Assert.AreEqual(strategy.ShouldSelectNote(null, barIndex, noteIndex), (noteIndex+1) % Mathf.Max(1,everyNNote) == 0);
    }
}
