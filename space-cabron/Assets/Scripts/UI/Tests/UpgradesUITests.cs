using System.Collections;
using Frictionless;
using Gmap.Gameplay;
using NUnit.Framework;
using SpaceCabron.Gameplay.Interactables;
using SpaceCabron.Messages;
using SpaceCabron.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static LevelTests;
using static SpaceCabron.Messages.MsgSpawnDrone;

public class UpgradesUITests
{
    GameObject eventHandlers;
    GameObject playerInstance;
    GameObject upgradeInstance;
    InteractableBehaviour upgradeBehaviour;
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

    private void SetupBaseScene()
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
    }

    private IEnumerator SetupSceneWithUpgrade(GameObject upgradePrefab)
    {
        SetupBaseScene();

        this.upgradeInstance = GameObject.Instantiate(
            upgradePrefab, 
            new Vector3(0, 1, 0), 
            Quaternion.identity
        );
        yield break;
    }

    private IEnumerator SetupSceneWithUpgrade(Upgrade upgrade)
    {
        SetupBaseScene();

        this.upgradeInstance = Interactable.CreateInteractable(
            upgrade, Vector3.up
        );

        this.upgradeBehaviour = this.upgradeInstance.GetComponent<InteractableBehaviour>();
        yield break;
    }

    [UnityTest]
    public IEnumerator GettingUpgradeAddsIconToUI()
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

    [UnityTest]
    public IEnumerator BuyingUpgradeAddsItemToUI()
    {
        Upgrade upgrade = Resources.Load<Upgrade>("Upgrades/PlayerSpawnWithBreakNote");
        yield return SetupSceneWithUpgrade(upgrade);

        InteractableTests.SpawnResourcesToPayFor(this.playerInstance, upgrade);

        yield return InteractableTests.InteractWithInteractable(
            this.brain, 
            this.playerInstance, 
            this.upgradeBehaviour
        );

        yield return null;

        UIUpgradesInfo info = canvasInstance.GetComponentInChildren<UIUpgradesInfo>();
        Assert.AreEqual(1, info.transform.childCount);
    }

    [UnityTest]
    public IEnumerator DyingAfterBuyingUpgradeMaintainsUpgradeOnScreen()
    {
        Upgrade upgrade = Resources.Load<Upgrade>("Upgrades/PlayerSpawnWithBreakNote");
        yield return SetupSceneWithUpgrade(upgrade);

        InteractableTests.SpawnResourcesToPayFor(this.playerInstance, upgrade);

        yield return InteractableTests.InteractWithInteractable(
            this.brain, 
            this.playerInstance, 
            this.upgradeBehaviour
        );

        yield return null;

        UIUpgradesInfo info = canvasInstance.GetComponentInChildren<UIUpgradesInfo>();
        Assert.AreEqual(1, info.transform.childCount);

        MessageRouter.RaiseMessage(new MsgSpawnDrone {
            DroneType = EDroneType.EveryN,
            Player = this.playerInstance
        });

        yield return null;

        this.playerInstance.GetComponent<Health>().Destroy();

        yield return new WaitForSeconds(2f);

        Assert.AreEqual(1, info.transform.childCount);
    }
}
