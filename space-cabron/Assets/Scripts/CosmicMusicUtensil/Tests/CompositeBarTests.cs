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

    [Test]
    public void Run()
    {
        
        Debug.Log( new Solution().solution(new int[] { 2, 8, 4, 3, 2 }, 7, 11, 3));
    }
 

// using System;
// using System.Linq;

public class FuelDispenser
{
    public int TimeStartedFueling = int.MinValue;
    public int CurrentFuel = 0;
    public int CurrentCarFuelNeeded = 0;

    public bool CanFuel(int t, int fuelNeeded) 
    { 
        return CurrentCarFuelNeeded == 0 && CurrentFuel > fuelNeeded;
    }
        
    public void Update() {
        if (CurrentCarFuelNeeded == 0)
            return;

        CurrentFuel--;
        CurrentCarFuelNeeded--;
    }
}

class Solution {

    int currentTime;

    bool TimeStep(FuelDispenser[] dispensers, int currentCar, out bool nextCar) {
        foreach (FuelDispenser d in dispensers) {
            d.Update();
        }
        
        Debug.Log($"Current Car: {currentCar}");
        Debug.Log($"D1: {dispensers[0].CurrentFuel} {dispensers[0].CurrentCarFuelNeeded}");
        Debug.Log($"D2: {dispensers[1].CurrentFuel} {dispensers[1].CurrentCarFuelNeeded}");
        Debug.Log($"D3: {dispensers[2].CurrentFuel} {dispensers[2].CurrentCarFuelNeeded}");
     

        currentTime++;

        nextCar = false;
        if (dispensers.All(d=>d.CurrentFuel < currentCar))
            return false;

        var availableDispenser = dispensers.FirstOrDefault(d => d.CanFuel(currentTime, currentCar) );
        if (availableDispenser == null) {
            return true;
        }
        
        nextCar = true;
        availableDispenser.CurrentCarFuelNeeded = currentCar;
        availableDispenser.TimeStartedFueling = currentTime;
        return true;
    }

    public int solution(int[] A, int X, int Y, int Z) {
        FuelDispenser[] d = {
            new FuelDispenser { CurrentFuel = X },
            new FuelDispenser { CurrentFuel = Y },
            new FuelDispenser { CurrentFuel = Z }
        };


        int maxWaitingTime = -2;
        int carWaitingTime = -1;
        currentTime = -1;
        int currentCarIndex = 0;
        bool nextCar = false;
        bool run = true;

        while (run) 
        {
            carWaitingTime++;
            run = TimeStep(d, A[currentCarIndex], out nextCar);
            Debug.Log($"{nextCar}");
            if (nextCar) {
                currentCarIndex++;
                maxWaitingTime = Mathf.Max(carWaitingTime, maxWaitingTime);
                carWaitingTime = -1;
                if (currentCarIndex >= A.Length-1)
                   return maxWaitingTime;
            }
        }
        return -1;
    }
}
    


}
