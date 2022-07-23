using Gmap.CosmicMusicUtensil;
using NUnit.Framework;


public class MelodyTests
{

    [Test]
    public static void MelodyFromNotes()
    {
        Note[] Notes = { new Note(ENote.C, 8, 3) };
        Melody m = new Melody(Notes);
        Assert.AreEqual(m.Notation, "c3/8");
    }



    [TestCase("c4/4", 1, ExpectedResult="c#4/4")]
    [TestCase("c4/4", -2, ExpectedResult="a#3/4")]
    [TestCase("c4/4", 12, ExpectedResult="c5/4")]
    [TestCase("c4/4;d4/4;e3/4", 2, ExpectedResult="d4/4;e4/4;f#3/4")]
    public static string MelodyTranspose(string notation, int steps)
    {
        Melody m = new Melody(notation);
        m.Transpose(steps);
        return m.Notation;
    }
}