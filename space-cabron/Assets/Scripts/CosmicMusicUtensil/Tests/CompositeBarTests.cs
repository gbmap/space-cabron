using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CompositeBarTests
{
    public static CompositeBar Create()
    {
        IModifiableBar bar = Bar.Create() as IModifiableBar;
        bar.AddNote(new Note { Tone = ENote.C, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.D, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.E, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.F, Interval = 4 });

        CompositeBar comp = CompositeBar.Create();
        comp.AddBar(bar as IBar);

        return comp;
    }

    // A Test behaves as an ordinary method
    [Test]
    public void TestGetNoteSingleBar()
    {
        IModifiableBar bar = Bar.Create() as IModifiableBar;
        bar.AddNote(new Note { Tone = ENote.C, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.D, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.E, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.F, Interval = 4 });

        CompositeBar comp = CompositeBar.Create();
        comp.AddBar(bar as IBar);
        Assert.AreEqual(comp.GetNote(0).Tone, ENote.C);
        Assert.AreEqual(comp.GetNote(1).Tone, ENote.D);
        Assert.AreEqual(comp.GetNote(2).Tone, ENote.E);
        Assert.AreEqual(comp.GetNote(3).Tone, ENote.F);
    }

    [Test]
    public void TestGetNoteMultipleBars()
    {
        IModifiableBar bar = Bar.Create() as IModifiableBar;
        bar.AddNote(new Note { Tone = ENote.C, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.D, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.E, Interval = 4 });
        bar.AddNote(new Note { Tone = ENote.F, Interval = 4 });

        IModifiableBar bar2 = Bar.Create() as IModifiableBar;
        bar2.AddNote(new Note { Tone = ENote.G, Interval = 4 });
        bar2.AddNote(new Note { Tone = ENote.A, Interval = 4 });
        bar2.AddNote(new Note { Tone = ENote.B, Interval = 4 });
        bar2.AddNote(new Note { Tone = ENote.Csharp, Interval = 4 });

        CompositeBar comp = CompositeBar.Create();
        comp.AddBar(bar as IBar);
        comp.AddBar(bar2 as IBar);

        Assert.AreEqual(comp.GetNote(0).Tone, ENote.C);
        Assert.AreEqual(comp.GetNote(1).Tone, ENote.D);
        Assert.AreEqual(comp.GetNote(2).Tone, ENote.E);
        Assert.AreEqual(comp.GetNote(3).Tone, ENote.F);
        Assert.AreEqual(comp.GetNote(4).Tone, ENote.G);
        Assert.AreEqual(comp.GetNote(5).Tone, ENote.A);
        Assert.AreEqual(comp.GetNote(6).Tone, ENote.B);
        Assert.AreEqual(comp.GetNote(7).Tone, ENote.Csharp);
    }

    [Test]
    public void TestGetMultipleBarRandomNotes()
    {
        CompositeBar c = CompositeBar.Create();
        int interval = 10;

        Note[] n = Enumerable.Range(0, 100).Select(
            v=> new Note {
                Tone = (ENote)Random.Range(0, ((int)ENote.B)+1),
                Interval = interval
            }
        ).ToArray();
        for (int i = 0; i < interval; i++)
        {
            IModifiableBar b = Bar.Create() as  IModifiableBar;
            n.Skip(i*10)
             .Take(interval)
             .ToList()
             .ForEach(n=>b.AddNote(n));

            c.AddBar(b as IBar);
        }

        for (int i = 0; i < 999; i++)
        {
            int index = Random.Range(0, n.Length);
            Note n1 = c.GetNote(index);
            Note n2 = n[index];

            Assert.AreEqual(n1.Tone, n2.Tone);
        }
    }

    [Test]
    public void Test_NegativeNoteIndex()
    {
        CompositeBar b = Create();
        Note n = b.GetNote(-1);
        Assert.AreEqual(n.Tone, ENote.F);
    }
}
