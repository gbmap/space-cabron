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

public class PlayerTests
{
    private bool finishedLoading;

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
        yield return new WaitForSeconds(5.0f);

        MessageRouter.RaiseMessage(new MsgLevelWon {});
        yield return new WaitForSeconds(3.0f);

        var player = GameObject.FindGameObjectWithTag("Player");
        Assert.AreEqual(1, player.GetComponentsInChildren<VictoryBrain>().Length);

        yield return new WaitForSeconds(1f);

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
            Player = player , 
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
}
