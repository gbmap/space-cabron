using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using Frictionless;
using Gmap;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using SpaceCabron.Gameplay;

public class LevelTests
{
    public static TestCaseData[] GetLevels()
    {
        // return new TestCaseData[] {
        //     new TestCaseData(
        //         AssetDatabase.LoadAssetAtPath<LevelConfiguration>("Assets/Data/Levels/Level0.asset")
        //     ).Returns(null)
        // };

        var objs = Resources.LoadAll<LevelConfiguration>("Levels/");
        return objs.Select(o => new TestCaseData(o).Returns(null)).ToArray();
    }

    public static TestCaseData[] GetSingleLevel()
    {
        return new TestCaseData[] {
            new TestCaseData(
                Resources.Load<LevelConfiguration>("Levels/Level0")
            ).Returns(null)
        };
    }

    [Test, TestCaseSource(nameof(GetLevels))]
    public IEnumerator LoadLevelLoadsLevelInformation(LevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(1f);
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        Assert.AreEqual(spawner.ScoreThreshold, level.Gameplay.ScoreThreshold);
        Assert.AreEqual(spawner.EnemyPool, level.Gameplay.EnemyPool);
        Assert.AreEqual(spawner.BossPool, level.Gameplay.BossPool);
        Assert.AreEqual(RenderSettings.skybox, level.Background.Material);
    }


    [UnityTest]
    [TestCaseSource(nameof(GetLevels))]
    public IEnumerator NextLevelMessageLoadsNextLevelCorrectly(LevelConfiguration level)
    {
        if (level.NextLevel != null)
        {
            LevelLoader.Load(level);
            yield return new WaitForSecondsRealtime(1f);

            MessageRouter.RaiseMessage(new MsgLevelWon());

            // Wait win animations.
            yield return new WaitForSecondsRealtime(5f);
            Assert.AreEqual(level.NextLevel, LevelLoader.CurrentLevelConfiguration);
        }
    }

    [UnityTest]
    [TestCaseSource(nameof(GetSingleLevel))]
    public IEnumerator PlayerWinsWhenScoreReachesThreshold(LevelConfiguration level)
    {
        bool hasLoaded = false;
        MessageRouter.AddHandler<MsgLevelFinishedLoading>((msg)=>hasLoaded=true);
        LevelLoader.Load(level);
        while (!hasLoaded)
            yield return null;
        
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        spawner.shouldSpawn = false;
        yield return new WaitForSeconds(1f);
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));
        yield return new WaitForSeconds(5f);
        Assert.AreEqual(LevelLoader.CurrentLevelConfiguration, level.NextLevel);
    }
}