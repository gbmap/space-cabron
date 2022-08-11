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
using Gmap.CosmicMusicUtensil;
using UnityEngine.SceneManagement;

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

    [TearDown]
    public void TearDown()
    {
        Unload();
    }

    public static void Unload()
    {
        for (int i = 1; i < SceneManager.sceneCount; i++)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
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
        yield return LoadLevelAndWait(level);
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        spawner.shouldSpawn = false;

        var player = GameObject.FindWithTag("Player");
        player.GetComponent<Health>().CanTakeDamage = false;

        MessageRouter.RaiseMessage(new MsgLevelWon());

        // Wait win animations.
        yield return new WaitForSecondsRealtime(1f);
        Assert.AreEqual(level.NextLevel, LevelLoader.CurrentLevelConfiguration);
    }

    private IEnumerator LoadLevelAndWait(LevelConfiguration level)
    {
        bool hasLoaded = false;
        LevelLoader.Load(level, () => hasLoaded = true);
        while (!hasLoaded)
            yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerWinsWhenScoreReachesThreshold()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);
        
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        spawner.shouldSpawn = false;
        yield return new WaitForSeconds(1f);
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));
        yield return new WaitForSeconds(12f);
        Assert.AreEqual(LevelLoader.CurrentLevelConfiguration, level.NextLevel);
    }

    [UnityTest]
    public IEnumerator PlayerDoesntWinWhenScoreReachesThresholdAndEnemiesExist()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);

        var player = GameObject.FindWithTag("Player");
        player.GetComponent<Health>().CanTakeDamage = false;
        player.GetComponentInChildren<TurntableBehaviour>().enabled = false;
        yield return new WaitForSeconds(7f);
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));
        yield return new WaitForSeconds(12f);
        Assert.AreEqual(LevelLoader.CurrentLevelConfiguration, level);
    }

    [UnityTest]
    public IEnumerator PlayerWinsWhenEnemiesGoOutOfScreen()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);

        var player = GameObject.FindWithTag("Player");
        player.GetComponent<Health>().CanTakeDamage = false;
        player.GetComponentInChildren<TurntableBehaviour>().enabled = false;
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        spawner.shouldSpawn = false;
        var enemyPrefab = Resources.Load<GameObject>("Enemies/EnemyThug");
        var enemyInstance = GameObject.Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));

        float time = 0f;
        while (enemyInstance != null)
        {
            time += Time.deltaTime;
            if (time > 10f)
                break;
            // Assert.AreEqual(LevelLoader.CurrentLevelConfiguration, level);
            yield return null;
        }
        yield return new WaitForSeconds(8f);

        Assert.AreEqual(level.NextLevel, LevelLoader.CurrentLevelConfiguration);
    }

    [UnityTest]
    public IEnumerator WinningLevelLoadsNextLevelWithoutLoadingNewScene()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        spawner.shouldSpawn = false;

        Scene s = SceneManager.GetSceneAt(1);
        int sceneCount = SceneManager.sceneCount;
        SceneManager.SetActiveScene(s);

        var player = GameObject.FindWithTag("Player");
        player.GetComponent<Health>().CanTakeDamage = false;
        player.GetComponentInChildren<TurntableBehaviour>().enabled = false;
        Vector3 playerPosition = player.transform.position;


        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));

        yield return new WaitForSeconds(3f);

        Assert.AreEqual(sceneCount, SceneManager.sceneCount);
        Assert.AreEqual(s, SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        Assert.IsNotNull(player);
        Assert.AreEqual(playerPosition, player.transform.position);

        Assert.AreEqual(level.NextLevel, LevelLoader.CurrentLevelConfiguration);
    }
}