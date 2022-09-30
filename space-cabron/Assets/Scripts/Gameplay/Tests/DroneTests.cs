using System.Collections;
using Frictionless;
using Gmap;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Gun;
using NUnit.Framework;
using SpaceCabron.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static SpaceCabron.Messages.MsgSpawnDrone;

public class DroneTests  : DefaultTestScene
{
    [UnityTest]
    public IEnumerator SpawningDroneGeneratesMelodyWithSameRootAndScale()
    {
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
        yield return new WaitForSeconds(2.0f);

        TurntableResolver resolver = TurntableResolver.Create("GlobalInstruments", "PlayerInstrument");

        var player = GameObject.FindGameObjectWithTag("Player");
        var playerTurntable = resolver.Get();
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
        PlayerTests.SpawnPlayer();
        yield return new WaitForSeconds(2f);
    }

    [
        UnityTest, 
        TestCase("PlayerBot", ExpectedResult=null), 
        TestCase("PlayerBotMelody", ExpectedResult=null), 
        TestCase("PlayerBotEveryN", ExpectedResult=null)
    ]
    public IEnumerator SpawningDroneThrowsNoException(string droneType)
    {
        GameObject dronePrefab = Resources.Load<GameObject>(droneType);
        var instance = GameObject.Instantiate(dronePrefab);
        yield return new WaitForSeconds(2f);
    }

    [
        UnityTest, 
        TestCase("PlayerBot", ExpectedResult=null), 
        TestCase("PlayerBotMelody", ExpectedResult=null), 
        TestCase("PlayerBotEveryN", ExpectedResult=null)
    ]
    public IEnumerator SpawningDroneWithPlayerThrowsNoException(string drone)
    {
        GameObject dronePrefab = Resources.Load<GameObject>(drone);
        var instancePlayer = SpawnPlayer();
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
    public IEnumerator DestroyingPlayerWithDroneRespawnsPlayer()
    {
        var player = SpawnPlayer();
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
        var player = PlayerTests.SpawnPlayer();
        PlayerTests.MakePlayerInvincible();
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

        Health health = player.GetComponent<Health>();
        if (health != null) {
            health.Destroy();
        }
        yield return new WaitForSeconds(5f);

        var player2 = GameObject.FindGameObjectWithTag("Player");
        Assert.IsTrue(player2 != null);
        Assert.AreNotEqual(player, player2);
        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("Player").Length);
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

    [
        UnityTest, 
        TestCase(EDroneType.Random, ExpectedResult=null), 
        TestCase(EDroneType.Melody, ExpectedResult=null), 
        TestCase(EDroneType.EveryN, ExpectedResult=null)
    ]
    public IEnumerator DronesContinueFiringAfterPlayerRespawns(EDroneType drone)
    {
        var player = SpawnPlayer();
        GameObject droneInst = null;

        MessageRouter.RaiseMessage(new MsgSpawnDrone
        {
            DroneType = drone,
            OnSpawned = (GameObject droneInstance) => {
                droneInst = droneInstance;
            }
        });

        while (droneInst == null)
            yield return null;

        if (drone == EDroneType.Random)
        {
            RepeatNoteWithStep s = droneInst.GetComponent<RepeatNoteWithStep>();
            s.Probability = 1f;
            s.SelectionStrategy = ScriptableObject.CreateInstance<ScriptableAllSelection>();
        }

        GameObject droneInst2 = null;
        MessageRouter.RaiseMessage(new MsgSpawnDrone
        {
            DroneType = drone,
            OnSpawned = (GameObject droneInstance) => {
                droneInst2 = droneInstance;
            }
        });

        while (droneInst2 == null)
            yield return null;
        
        if (drone == EDroneType.Random)
        {
            RepeatNoteWithStep s = droneInst2.GetComponent<RepeatNoteWithStep>();
            s.Probability = 1f;
            s.SelectionStrategy = ScriptableObject.CreateInstance<ScriptableAllSelection>();
        }

        yield return new WaitForSeconds(1f);
        player.GetComponent<Health>().Destroy();
        yield return new WaitForSeconds(1f);
        do
        {
            player = GameObject.FindGameObjectWithTag("Player");
        } while (player == null);

        if (droneInst == null)
            droneInst = droneInst2;

        float time = 10f;
        yield return new WaitForSeconds(time);

        GunBehaviour gun = droneInst.GetComponent<GunBehaviour>();
        Assert.IsNotNull(gun.LastShot);

        float lastShot = gun.LastShot.Time;
        Assert.IsTrue(Mathf.Abs(Time.time - lastShot) < time);
    }

}
