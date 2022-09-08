using System.Linq;
using System.Collections;
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
using SpaceCabron.Gameplay.Level;
using SpaceCabron.Gameplay.Interactables;
using Gmap.ScriptableReferences;
using static SpaceCabron.Messages.MsgSpawnDrone;

public class LevelTests
{
    public class PlayerControlBrain : ScriptableAIBrain<InputState>
    {
        public Vector2 Movement = new Vector2();
        public bool Shoot = false;
        public override InputState GetInputState(InputStateArgs args)
        {
            return new InputState
            {
                Movement = this.Movement,
                Shoot = this.Shoot
            };
        }
    }

    public static TestCaseData[] GetEnemyLevels()
    {
        var objs = Resources.LoadAll<LevelConfiguration>("Levels/");
        return objs.Select(o => new TestCaseData(o).Returns(null)).ToArray();
    }

    public static TestCaseData[] GetBufferLevels()
    {
        var objs = Resources.LoadAll<BufferLevelConfiguration>("Levels/");
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

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        System.Array.ForEach(enemies, go => GameObject.Destroy(go));

        var bullets = GameObject.FindGameObjectsWithTag("Untagged")
                                .Where(go => go.layer == LayerMask.NameToLayer("Bullet"))
                                .ToArray();
        System.Array.ForEach(bullets, go => GameObject.Destroy(go));
    }

    [Test, TestCaseSource(nameof(GetEnemyLevels))]
    public IEnumerator LoadLevelLoadsLevelInformation(LevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(1f);
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        Assert.AreEqual(spawner.ScoreThreshold, level.Gameplay.ScoreThreshold);
        Assert.AreEqual(spawner.EnemyPool, level.Gameplay.EnemyPool);
        Assert.AreEqual(RenderSettings.skybox, level.Background.Material);
    }


    [UnityTest]
    [TestCaseSource(nameof(GetEnemyLevels))]
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

    [UnityTest]
    [TestCaseSource(nameof(GetBufferLevels))]
    public IEnumerator LoadBufferLevelsLoadsLevelsCorrectly(BufferLevelConfiguration level)
    {
        yield return LoadLevelAndWait(level);

        InteractableBehaviour[] interactables = GameObject.FindObjectsOfType<InteractableBehaviour>();
        int interactableCount = level.GetNumberOfUpgrades() + 1;
        Assert.AreEqual(interactableCount, interactables.Length);
        Assert.AreEqual(1, interactables.Where(i => i.Interactable is NextLevelInteractable).Count());
    }

    public static IEnumerator LoadLevelAndWait(BaseLevelConfiguration level)
    {
        bool hasLoaded = false;
        LevelLoader.Load(level, () => hasLoaded = true);
        while (!hasLoaded)
            yield return null;
    }
    
    public static EnemySpawner DisableEnemySpawner()
    {
        EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
        if (spawner != null)
            spawner.shouldSpawn = false;
        return spawner;
    }

    public static GameObject MakePlayerInvincible()
    {
        var player = GameObject.FindWithTag("Player");
        player.GetComponent<Health>().CanTakeDamage = false;
        player.GetComponentInChildren<TurntableBehaviour>().enabled = false;
        return player;
    }

    public static PlayerControlBrain InjectPlayerWithControlBrain(GameObject player)
    {
        PlayerControlBrain brain = ScriptableObject.CreateInstance<PlayerControlBrain>();
        GameObject.Destroy(player.GetComponent<InjectBrainToActor<InputState>>());
        InjectBrainToActor<InputState>.Inject(player, brain);
        return brain;
    }

    [UnityTest]
    public IEnumerator PlayerWinsWhenScoreReachesThreshold()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);
        DisableEnemySpawner();
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
        MakePlayerInvincible();
        yield return new WaitForSeconds(10f);
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));
        yield return new WaitForSeconds(12f);
        Assert.AreEqual(LevelLoader.CurrentLevelConfiguration, level);
    }

    [UnityTest]
    public IEnumerator PlayerWinsWhenEnemiesGoOutOfScreen()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);
        DisableEnemySpawner();
        MakePlayerInvincible();

        var enemyPrefab = Resources.Load<GameObject>("Enemies/EnemyThug");
        var enemyInstance = GameObject.Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));

        float time = 0f;
        while (enemyInstance != null)
        {
            time += Time.deltaTime;
            if (time > 30f)
            {
                Assert.Fail("Enemy didn't go out of screen.");
                break;
            }
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
        var spawner = DisableEnemySpawner();

        Scene s = SceneManager.GetSceneAt(1);
        int sceneCount = SceneManager.sceneCount;
        SceneManager.SetActiveScene(s);

        var player = MakePlayerInvincible();
        Vector3 playerPosition = player.transform.position;

        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));

        yield return new WaitForSeconds(3f);

        Assert.AreEqual(sceneCount, SceneManager.sceneCount);
        Assert.AreEqual(s, SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        Assert.IsNotNull(player);
        Assert.AreEqual(playerPosition, player.transform.position);

        Assert.AreEqual(level.NextLevel, LevelLoader.CurrentLevelConfiguration);

        if (level.NextLevel is LevelConfiguration)
        {
            var nextLevel = level.NextLevel as LevelConfiguration;
            Assert.AreEqual(nextLevel.Gameplay.ScoreThreshold, spawner.ScoreThreshold);
        }
    }

    [UnityTest]
    public IEnumerator MsgSpawnPlayerSpawnsOnlyOnePlayerAfterLevelLoads()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);

        DisableEnemySpawner();
        var player = MakePlayerInvincible();
        var brain = PlayerControlBrain.CreateInstance<PlayerControlBrain>();
        brain.Shoot = false;
        InjectBrainToActor<InputState>.Inject(player, brain);

        for (int i = 0; i < 3; i++)
        {
            MessageRouter.RaiseMessage(new MsgSpawnDrone {
                DroneType = MsgSpawnDrone.EDroneType.Melody,
                Player = player
            });
        }

        // Win level (Level0Buffer)
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue/2, int.MaxValue/2));
        yield return null;

        // Go to next level (Level1)
        InteractableBehaviour interactable = GameObject.FindObjectsOfType<InteractableBehaviour>()
                                                       .FirstOrDefault(i => i.Interactable is NextLevelInteractable);
        yield return InteractableTests.InteractWithInteractable(
            brain, player, interactable
        );
        yield return new WaitForSeconds(1.0f);

        // Win level (Level1Buffer)
        DisableEnemySpawner();
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, int.MaxValue));

        yield return new WaitForSeconds(0.5f);

        // Go to next level (Level2)
        interactable = GameObject.FindObjectsOfType<InteractableBehaviour>()
                                                       .FirstOrDefault(i => i.Interactable is NextLevelInteractable);
        yield return InteractableTests.InteractWithInteractable(
            brain, player, interactable
        );

        yield return null;

        // MessageRouter.RaiseMessage(new MsgOnScoreChanged(0, 0));
        DisableEnemySpawner();

        player.GetComponent<Health>().Destroy();

        yield return new WaitForSeconds(5f);

        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("Player").Length);

    }

    [UnityTest]
    public IEnumerator PlayerSpawnsWithTwoDronesOnNextLevel()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);

        DisableEnemySpawner();
        var player = MakePlayerInvincible();
        var brain = PlayerControlBrain.CreateInstance<PlayerControlBrain>();
        brain.Shoot = false;
        InjectBrainToActor<InputState>.Inject(player, brain);

        var drones = GameObject.FindGameObjectsWithTag("Drone");
        for (int i = 0; i < drones.Length; i++)
            GameObject.Destroy(drones[i]);

        // Win level (Level0Buffer)
        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue/2, int.MaxValue/2));
        yield return null;

        InteractableBehaviour interactable = GameObject.FindObjectsOfType<InteractableBehaviour>()
                                                       .FirstOrDefault(i => i.Interactable is NextLevelInteractable);
        yield return InteractableTests.InteractWithInteractable(
            brain, player, interactable
        );
        yield return new WaitForSeconds(1.0f);

        drones = GameObject.FindGameObjectsWithTag("Drone");
        Assert.AreEqual(2, drones.Length);
    }

    [UnityTest]
    public IEnumerator PlayerSpawnsWithTwoDronesOnRespawn()
    {
        LevelConfiguration level = Resources.Load<LevelConfiguration>("Levels/Level0");
        yield return LoadLevelAndWait(level);

        DisableEnemySpawner();
        var player = MakePlayerInvincible();
        var brain = PlayerControlBrain.CreateInstance<PlayerControlBrain>();
        brain.Shoot = false;
        InjectBrainToActor<InputState>.Inject(player, brain);

        player.GetComponent<Health>().Destroy();

        GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
        for (int i = 0; i < drones.Length; i++)
            GameObject.Destroy(drones[i]);

        yield return new WaitForSeconds(12f);

        GameOverMenu menu = GameObject.FindObjectOfType<GameOverMenu>();
        menu.Retry();

        yield return new WaitForSeconds(3f);
        player = MakePlayerInvincible();

        Assert.AreEqual(2, GameObject.FindGameObjectsWithTag("Drone").Length);
    }
}