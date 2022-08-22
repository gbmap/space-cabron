using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using NUnit.Framework;
using UnityEngine;

public class ImprovisationTests 
{
    [Test]
    public static void ImprovisationEqualsItself()
    {
        Improvisation improvisationA = new BreakNoteImprovisation(
            new SelectAllStrategy(),
            new SelectAllStrategy()
        );

        Assert.AreEqual(improvisationA, improvisationA);
    }

    [Test]
    public static void ImprovisationDiffersOnInstance()
    {
        Improvisation improvisationA = new BreakNoteImprovisation(
            new SelectAllStrategy(),
            new SelectAllStrategy()
        );

        Improvisation improvisationB = new BreakNoteImprovisation(
            new SelectAllStrategy(),
            new SelectAllStrategy()
        );

        Assert.AreNotEqual(improvisationA, improvisationB);
    }
}
