using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using SpaceCabron.Gameplay.Multiplayer;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using System.Linq;
using Gmap.CosmicMusicUtensil;
using System;
using Gmap;
using Frictionless;
using SpaceCabron.Messages;

public class MultiplayerTests 
{
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("Player"), p => GameObject.Destroy(p));
        MultiplayerManager.PlayerCount = 1;
        yield break;
    }

    private IEnumerator LoadLevel(int nPlayers=2)
    {
        MultiplayerManager.PlayerCount = nPlayers;
        LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level1"));
        yield return new WaitForSeconds(2f);
    }

    private void DestroyNPlayers(int n)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        System.Array.ForEach(players.Take(n).ToArray(), p=>p.GetComponent<Health>().Destroy());
    }


    [UnityTest, TestCase(1, ExpectedResult=null), TestCase(2, ExpectedResult=null)]
    public IEnumerator MultiplayerSpawnsCorrectLevelOfPlayers(int nPlayers)
    {
        yield return LoadLevel(nPlayers);
        Assert.AreEqual(nPlayers, GameObject.FindGameObjectsWithTag("Player").Length);
    }

    // [UnityTest]
    // public IEnumerator MultiplePlayersSpawnWithTheSameMelody()
    // {
    //     yield return LoadLevel(2);

    //     GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //     var melodies = players.Select(p=>p.GetComponentInChildren<ITurntable>().Melody);
    //     Assert.IsTrue(melodies.All(m => m.Notation == melodies.First().Notation));
    // }

    [UnityTest]
    public IEnumerator OnePlayerDeathDoesntTransitionToGameOver()
    {
        yield return LoadLevel(2);
        LevelTests.DisableEnemySpawner();
        PlayerTests.DestroyAllDrones();

        yield return new WaitForSeconds(2f);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        players[0].GetComponent<Health>().Destroy();

        yield return new WaitForSeconds(7f);

        Assert.AreNotEqual(Resources.Load<GameState>("GameStates/GameplayGameOverMenu"), GameState.Current);
    }

    [UnityTest]
    public IEnumerator TwoPlayerDeathsTransitionToGameOver()
    {
        yield return LoadLevel(2);
        LevelTests.DisableEnemySpawner();
        PlayerTests.DestroyAllDrones();

        yield return new WaitForSeconds(2f);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        System.Array.ForEach(players, p => p.GetComponent<Health>().Destroy());

        yield return new WaitForSeconds(15f);

        Assert.AreEqual(Resources.Load<GameState>("GameStates/GameplayGameOverMenu"), GameState.Current);
    }

    [UnityTest]
    public IEnumerator OnePlayerWinSpawnsOtherPlayer()
    {
        yield return LoadLevel(2);
        yield return new WaitForSeconds(2f);
        LevelTests.DisableEnemySpawner();
        PlayerTests.DestroyAllDrones();

        yield return new WaitForSeconds(3f);
        
        PlayerTests.DestroyAllEnemies();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        players[0].GetComponent<Health>().Destroy();

        yield return new WaitForSeconds(7f);

        MessageRouter.RaiseMessage(new MsgOnScoreChanged(int.MaxValue, 1));

        yield return new WaitForSeconds(7f);

        Assert.AreEqual(2, GameObject.FindGameObjectsWithTag("Player").Length);
    }

    [UnityTest]
    public IEnumerator RetrySpawnsBothPlayers()
    {
        yield return LoadLevel(2);
        LevelTests.DisableEnemySpawner();
        yield return new WaitForSeconds(5f);
        PlayerTests.DestroyAllDrones();
        DestroyNPlayers(2);
        yield return new WaitForSeconds(15f);

        LevelTests.RetryGame();

        yield return new WaitForSeconds(2f);
        Assert.AreEqual(2, GameObject.FindGameObjectsWithTag("Player").Length);
    }

    [UnityTest]
    public IEnumerator BuyingPermanentImprovisationChangesBothPlayers()
    {
        Assert.Fail("Not implemented.");
        yield break;
    }
}