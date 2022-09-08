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
using SpaceCabron.Gameplay.Multiplayer;
using System.Linq;

public class DefaultTestScene
{
    protected GameObject eventHandlers;

    protected void DestroyWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        System.Array.ForEach(objects, o => GameObject.Destroy(o));
    }

    [SetUp]
    public void Init()
    {
        if (GameObject.FindObjectOfType<DroneSpawner>() == null)
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

        if (eventHandlers != null)
            GameObject.Destroy(eventHandlers);
    }

    public static GameObject SpawnPlayer()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        var instance = GameObject.Instantiate(playerPrefab);
        instance.name = "Player0";
        return instance;
    }
}

public class PlayerTests : DefaultTestScene
{
    private bool finishedLoading;

    [UnityTest]
    public IEnumerator BeginAnimationMovesPlayerTowardsPosition()
    {
        // yield return new WaitForSeconds(0.5f);
        var player = GameObject.Instantiate(
            Resources.Load<GameObject>("Player"), Vector3.down, Quaternion.identity
        );
        BeginLevelBrain.Play<BeginLevelBrain>(player);
        Assert.AreEqual(1, player.GetComponentsInChildren<BeginLevelBrain>().Length);

        yield return new WaitForSeconds(1f);

        Vector3 lastPos = player.transform.position;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Assert.AreNotEqual(lastPos, player.transform.position);
            lastPos = player.transform.position;
        }
        yield return new WaitForSeconds(5f);
        Assert.AreEqual(0, player.GetComponentsInChildren<BeginLevelBrain>().Length);
    }

    [UnityTest]
    public IEnumerator EndAnimationMovesPlayerTowardsPosition()
    {
        var player = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
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
