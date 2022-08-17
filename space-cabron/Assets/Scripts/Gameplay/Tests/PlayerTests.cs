using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using Gmap;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using SpaceCabron.Gameplay;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using UnityEngine.SceneManagement;

public class PlayerTests
{
    private bool finishedLoading;

    GameObject eventHandlers;

    [SetUp]
    public void Init()
    {
        eventHandlers = GameObject.Instantiate(Resources.Load<GameObject>("EventHandlers"));
    }

    [TearDown]
    public void TearDown()
    {
        System.Array.ForEach(
            new string[] {"Player", "Enemy", "Drone"}, 
            tag => DestroyWithTag(tag)
        );

        for (int i = 1; i < SceneManager.sceneCount; i++)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        GameObject.Destroy(eventHandlers);
    }

    private void DestroyWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        System.Array.ForEach(objects, o => GameObject.Destroy(o));
    }

    [UnityTest]
    public IEnumerator BeginAnimationMovesPlayerTowardsPosition()
    {
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
        yield return new WaitForSeconds(0.5f);
        var player = GameObject.FindGameObjectWithTag("Player");
        Assert.AreEqual(1, player.GetComponentsInChildren<BeginLevelBrain>().Length);

        Vector3 lastPos = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            Assert.AreNotEqual(lastPos, player.transform.position);
            lastPos = player.transform.position;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(5f);
        Assert.AreEqual(0, player.GetComponentsInChildren<BeginLevelBrain>().Length);
    }

    [UnityTest]
    public IEnumerator EndAnimationMovesPlayerTowardsPosition()
    {
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
        yield return new WaitForSeconds(6.0f);

        var player = GameObject.FindGameObjectWithTag("Player");
        VictoryBrain.Play<VictoryBrain>(player);
        yield return new WaitForSeconds(3.0f);

        Assert.AreEqual(1, player.GetComponentsInChildren<VictoryBrain>().Length);

        yield return new WaitForSeconds(2f);

        Vector3 lastPos = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            Assert.AreNotEqual(lastPos, player.transform.position);
            lastPos = player.transform.position;
            yield return new WaitForSeconds(0.1f);
        }
    }

    [UnityTest]
    public IEnumerator SpawningDroneGeneratesMelodyWithSameRootAndScale()
    {
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
        yield return new WaitForSeconds(2.0f);

        var player = GameObject.FindGameObjectWithTag("Player");
        var playerTurntable = player.GetComponentInChildren<ITurntable>();
        GameObject drone = null;
        MessageRouter.RaiseMessage(new MsgSpawnDrone 
        { 
            DroneType = MsgSpawnDrone.EDroneType.Melody, 
            Player = player, 
            OnSpawned = (GameObject droneInstance) => {
                drone = droneInstance;
            }
        });

        while (drone == null)
            yield return new WaitForSeconds(0.1f);

        var droneTurntable = drone.GetComponentInChildren<ITurntable>();
        Assert.AreEqual(playerTurntable.Melody.Root, droneTurntable.Melody.Root);
        Assert.AreEqual(playerTurntable.Melody.Scale, droneTurntable.Melody.Scale);
    }

    [UnityTest]
    public IEnumerator SpawningPlayerThrowsNoException()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        var instance = GameObject.Instantiate(playerPrefab);
        yield return new WaitForSeconds(2f);
    }

    [UnityTest]
    public IEnumerator SpawningDroneThrowsNoException()
    {
        GameObject dronePrefab = Resources.Load<GameObject>("PlayerBot");
        var instance = GameObject.Instantiate(dronePrefab);
        yield return new WaitForSeconds(2f);
    }

    [UnityTest]
    public IEnumerator SpawningDroneWithPlayerThrowsNoException()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        GameObject dronePrefab = Resources.Load<GameObject>("PlayerBot");
        var instancePlayer = GameObject.Instantiate(playerPrefab);
        var instance = GameObject.Instantiate(dronePrefab);
        yield return new WaitForSeconds(2f);
    }

    [UnityTest]
    public IEnumerator SpawningDroneThroughDroneSpawnerWithoutPlayerThrowsNoException()
    {
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
        yield return new WaitForSeconds(2f);
        MessageRouter.RaiseMessage(new MsgSpawnDrone { DroneType = MsgSpawnDrone.EDroneType.Melody });
    }

    [UnityTest]
    public IEnumerator SpawningDroneThroughDroneSpawnerWithPlayerThrowsNoException()
    {
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
        yield return new WaitForSeconds(2f);
        MessageRouter.RaiseMessage(new MsgSpawnDrone 
        { 
            DroneType = MsgSpawnDrone.EDroneType.Melody,
            Player = GameObject.FindGameObjectWithTag("Player")
        });
    }

    [UnityTest]
    public IEnumerator DestroyingPlayerSpawnsPlayerChip()
    {
        yield return new WaitForSeconds(0.1f);
        var player = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Health>().Destroy();
        yield return new WaitForSeconds(1f);
        Assert.IsNotNull(GameObject.FindGameObjectWithTag("PlayerChip"));
    }

    [UnityTest]
    public IEnumerator DestroyingPlayerWithoutDroneTransitionsToGameOver()
    {
        var player = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Health>().Destroy();
        yield return new WaitForSeconds(10f);
        GameState gameover = Resources.Load<Gmap.Gameplay.GameState>("GameStates/GameplayGameOverMenu");
        Assert.AreEqual(gameover, GameState.Current);
    }

    [UnityTest]
    public IEnumerator PlayerChipAndDroneSpawnsPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject.Instantiate(Resources.Load<GameObject>("PlayerChip"));

        MessageRouter.RaiseMessage(new MsgSpawnDrone {
            DroneType = MsgSpawnDrone.EDroneType.Random
        });

        yield return new WaitForSeconds(5f);

        Assert.IsNotNull(GameObject.FindGameObjectWithTag("Player"));
        GameObject.Destroy(GameObject.FindGameObjectWithTag("Player"));
        GameObject.Destroy(GameObject.FindGameObjectWithTag("PlayerChip"));
    }

    [UnityTest]
    public IEnumerator DestroyingPlayerWithDroneRespawnsPlayer()
    {
        var player = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
        yield return new WaitForSeconds(0.2f);
        MessageRouter.RaiseMessage(new MsgSpawnDrone 
        { 
            DroneType = MsgSpawnDrone.EDroneType.Melody,
            Player = player
        });
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Health>().Destroy();
        yield return new WaitForSeconds(5f);

        var player2 = GameObject.FindGameObjectWithTag("Player");
        Assert.IsTrue(player2 != null);
        Assert.AreNotEqual(player, player2);
    }

    [UnityTest]
    public IEnumerator CollisionWithMultipleDronesSpawnsOnlyOnePlayer()
    {
        var player = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i< 5; i++)
        {
            MessageRouter.RaiseMessage(new MsgSpawnDrone 
            { 
                DroneType = MsgSpawnDrone.EDroneType.Melody,
                Player = player
            });
        }
        yield return new WaitForSeconds(0.2f);

        GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");
        foreach (GameObject drone in drones)
        {
            FollowAtAnOffset f = drone.GetComponent<FollowAtAnOffset>();
            f.Offset = Vector3.up;
        }

        yield return new WaitForSeconds(2.0f);

        player.GetComponent<Health>().Destroy();
        yield return new WaitForSeconds(5f);

        var player2 = GameObject.FindGameObjectWithTag("Player");
        Assert.IsTrue(player2 != null);
        Assert.AreNotEqual(player, player2);
        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("Player").Length);
    }
}
