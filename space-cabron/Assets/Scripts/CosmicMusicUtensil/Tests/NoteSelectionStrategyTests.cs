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
    public bool EveryNSpecificCases(int everyNBar, int index)
    {
        SelectionStrategy strategy = new EveryNStrategy(everyNBar);
        return strategy.ShouldSelect(null, index);
    }


    [TestCase(2, 1)]
    [TestCase(2, 2)]
    [TestCase(0, 1)]
    [Test]
    public void RandomEveryN(
        [Random(0, 32, 10)] int everyNBar, 
        [Random(0, 100, 30)] int index
    ) {
        SelectionStrategy strategy = new EveryNStrategy(everyNBar);
        Assert.AreEqual(strategy.ShouldSelect(null, index), (index+1) % Mathf.Max(1, everyNBar) == 0);
    }
}
