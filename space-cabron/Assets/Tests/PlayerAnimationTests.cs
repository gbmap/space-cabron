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


public class PlayerAnimationTests
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
}
