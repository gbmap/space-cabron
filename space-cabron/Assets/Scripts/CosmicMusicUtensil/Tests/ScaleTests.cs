using Gmap.CosmicMusicUtensil;
using NUnit.Framework;

public class ScaleTests
{
    [Test]
    public static void TestSingleNoteScale()
    {
        Scale scale = new Scale {
            Intervals = new int[] { 0 }
        };

        for (int i = 0; i < 10; i++)
        {
            for (ENote n = ENote.C; n < ENote.None; n++)
                Assert.AreEqual(n, scale.GetNote(n, i));
        }
    }

    [Test]
    public static void TestEmptyScaleReturnsNone()
    {
        Scale scale = new Scale {
            Intervals = new int[] { }
        };

        Assert.AreEqual(ENote.None, scale.GetNote(ENote.C, 0));
        Assert.AreEqual(ENote.None, scale.GetNote(ENote.E, 0));
        Assert.AreEqual(ENote.None, scale.GetNote(ENote.D, 0));
        Assert.AreEqual(ENote.None, scale.GetNote(ENote.F, 0));
    }

    [Test]
    public static void TestScaleRepeatsNotes([Values(0, 1, 2, 5, 10, 128)] int nNotes)
    {
        int[] intervals = new int[nNotes];
        for ( int i = 0; i < nNotes; i++)
            intervals[i] = i;
        Scale scale = new Scale { Intervals = intervals };

        for (int i = -10; i < 10; i++) {
            for (int j = 0; j < nNotes; j++) {
                Assert.AreEqual(
                    scale.GetNote(ENote.C, j), 
                    scale.GetNote(ENote.C, j + scale.GetNumberOfNotes()*i)
                );
            }
        }
    }

}