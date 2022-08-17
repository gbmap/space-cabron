using System.Collections;
using Frictionless;
using Gmap.Gameplay;
using NUnit.Framework;
using SpaceCabron.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static LevelTests;

public class UpgradesUITests
{
    GameObject eventHandlers;
    GameObject playerInstance;
    GameObject upgradeInstance;
    GameObject canvasInstance;
    PlayerControlBrain brain;

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(canvasInstance);
        canvasInstance = null;

        GameObject.Destroy(playerInstance);
        playerInstance = null;

        MessageRouter.Reset();
    }

    private IEnumerator SetupSceneWithUpgrade(GameObject upgradePrefab)
    {
        var canvasGameplayPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Actor/UI/CanvasGameplay.prefab"
        );
        canvasInstance = GameObject.Instantiate(
            canvasGameplayPrefab, Vector3.zero, Quaternion.identity
        );

        var eventHandlerPrefab = Resources.Load<GameObject>("EventHandlers");
        this.eventHandlers = GameObject.Instantiate(eventHandlerPrefab);

        var playerPrefab = Resources.Load("Player");
        this.playerInstance = GameObject.Instantiate(
            playerPrefab, 
            Vector3.zero, 
            Quaternion.identity
        ) as GameObject;

        this.brain = InjectPlayerWithControlBrain(this.playerInstance);

        upgradeInstance = GameObject.Instantiate(
            upgradePrefab, 
            new Vector3(0, 1, 0), 
            Quaternion.identity
        );
        yield break;
    }

    [UnityTest]
    public IEnumerator GettingUpgradeAddsIconToList()
    {
        GameObject instance = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Actor/Upgrade/AddRandomImprovisation/AddRandomImprovisation.prefab"
        );

        yield return SetupSceneWithUpgrade(instance);

        while (upgradeInstance != null)
            yield return null;

        UIUpgradesInfo info = canvasInstance.GetComponentInChildren<UIUpgradesInfo>();
        Assert.AreEqual(1, info.transform.childCount);

        yield return new WaitForSeconds(1f);
    }

    [UnityTest]
    public IEnumerator PlayerDeathCleansTemporaryUpgrades()
    {
        GameObject instance = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Actor/Upgrade/AddRandomImprovisation/AddRandomImprovisation.prefab"
        );

        yield return SetupSceneWithUpgrade(instance);

        while (upgradeInstance != null)
            yield return null;

        UIUpgradesInfo info = canvasInstance.GetComponentInChildren<UIUpgradesInfo>();
        Assert.AreEqual(1, info.transform.childCount);

        yield return null;

        this.playerInstance.GetComponent<Health>().Destroy();

        yield return null;

        Assert.AreEqual(0, info.transform.childCount);

        yield return new WaitForSeconds(1f);
    }
}
