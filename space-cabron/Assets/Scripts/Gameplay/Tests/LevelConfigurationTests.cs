using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SpaceCabron.Gameplay;
using Gmap.CosmicMusicUtensil;

public class LevelConfigurationTests
{
    LevelConfiguration levelConfig;

    [SetUp]
    public void Init()
    {
        levelConfig = Resources.Load<LevelConfiguration>("Levels/TestLevel0");
    }

    // A Test behaves as an ordinary method
    [Test]
    public void CloningLevelConfigurationDoesntChangeOriginal()
    {
        var clone = levelConfig.Clone() as LevelConfiguration;
        var instrumentConfig = clone.GetInstrumentConfigurationByTag("Player");

        ScriptableMelodyFactory melodyFactory = ScriptableObject.CreateInstance<ScriptableRandomMelodyFactory>();
        instrumentConfig.MelodyFactory = melodyFactory;

        Assert.IsNotNull(levelConfig.GetInstrumentConfigurationByTag("Player").MelodyFactory);
    }

    [Test]
    public void CloningLevelAndSettingMelodyFactory()
    {
        Melody m = new Melody("c4/4;c4/4;c4/4;c4/4");
        if (m.IsEmpty)
            throw new System.Exception("Melody is empty.");

        ScriptableFixedMelodyFactory f = ScriptableObject.CreateInstance<ScriptableFixedMelodyFactory>();
        f.Notation = m.Notation.ToLower();

        var oldConfig = levelConfig;
        levelConfig = levelConfig.Clone() as LevelConfiguration;
        levelConfig.GetInstrumentConfigurationByTag("Player").MelodyFactory = f;

        Assert.AreNotSame(oldConfig, levelConfig);
        Assert.IsNotNull(oldConfig.GetInstrumentConfigurationByTag("Player").MelodyFactory);
    }
}
