using System.Collections;
using System.Linq;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using NUnit.Framework;
using SpaceCabron.Gameplay.Interactables;
using SpaceCabron.Gameplay.Level;
using SpaceCabron.Messages;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static LevelTests;
using static SpaceCabron.Messages.MsgSpawnDrone;

public class InteractableTests 
{
    [TearDown]
    public void TearDown()
    {
        MessageRouter.Reset();
        GameObject.Destroy(playerInstance);
        GameObject.Destroy(eventHandlers);

        var players = GameObject.FindGameObjectsWithTag("Player");
        System.Array.ForEach(players, player => GameObject.Destroy(player));

        var turntables = GameObject.FindObjectsOfType<TurntableBehaviour>();
        System.Array.ForEach(turntables, t => GameObject.Destroy(t.gameObject));

        var interactables = GameObject.FindObjectsOfType<InteractableBehaviour>();
        System.Array.ForEach(interactables, t => GameObject.Destroy(t.gameObject));

        var drones = GameObject.FindGameObjectsWithTag("Drone");
        System.Array.ForEach(drones, t => GameObject.Destroy(t.gameObject));

        Upgrade.Upgrades = new UpgradesContainer();
    }

    GameObject playerInstance;
    PlayerControlBrain brain;
    GameObject eventHandlers;
    GameObject interactableInstance;
    InteractableBehaviour interactableBehaviour;

    private void SetupInteractableTestScene(Interactable interactable)
    {
        var eventHandlerPrefab = Resources.Load<GameObject>("EventHandlers");
        this.eventHandlers = GameObject.Instantiate(eventHandlerPrefab);

        var playerPrefab = Resources.Load("Player");
        this.playerInstance = GameObject.Instantiate(
            playerPrefab, 
            Vector3.zero, 
            Quaternion.identity
        ) as GameObject;
        this.playerInstance.name += "0";

        this.brain = InjectPlayerWithControlBrain(this.playerInstance);

        this.interactableInstance = Interactable.CreateInteractable(
            interactable, new Vector3(0, 1, 0)
        );
        this.interactableBehaviour = interactableInstance.GetComponent<InteractableBehaviour>();
    }
    
    public static IEnumerator InteractWithInteractable(
        PlayerControlBrain brain,
        GameObject objectInstance,
        InteractableBehaviour interactableBehaviour
    ) {
        yield return MoveTo(brain, objectInstance, interactableBehaviour.transform.position);
        yield return new WaitForFixedUpdate();
        Assert.IsTrue(interactableBehaviour.IsSelected);
        yield return null;
        brain.Shoot = true;
        yield return null;
        brain.Shoot = false;
        yield break;
    }

    public static IEnumerator SpawnAndBuyImprovisationUpgrade(
        GameObject player, PlayerControlBrain brain, string upgradePath
    ) {
        // AddPermanentUpgrade();
        var upgrade = ScriptableObject.CreateInstance<PlayerRespawnsWithImprovisationUpgrade>();
        upgrade.Improvisation = AssetDatabase.LoadAssetAtPath<ScriptableImprovisation>(
            upgradePath
        );

        InteractableTests.SpawnResourcesToPayFor(player, upgrade);

        var interactable = Interactable.CreateInteractable(upgrade, Vector3.zero);
        yield return InteractableTests.InteractWithInteractable(
            brain, 
            player,
            interactable.GetComponent<InteractableBehaviour>()
        );
    }

    public static IEnumerator MoveTo(PlayerControlBrain brain, GameObject objectInstance, Vector3 targetPosition)
    {
        float time = 0f;
        while (Vector3.Distance(objectInstance.transform.position, targetPosition) > 0.05f && time < 10f)
        {
            brain.Movement = Vector3.ClampMagnitude(
                targetPosition - objectInstance.transform.position,
                1
            );
            yield return null;
            time += Time.deltaTime;
        }

        if (time >= 10f)
            Assert.Fail("Timed out while moving to target position");

        brain.Movement = Vector2.zero;
    }

    [UnityTest]
    public IEnumerator InteractableBehaviourRunsInteractOnScriptableObject()
    {
        NullInteractable interactable = ScriptableObject.CreateInstance<NullInteractable>();
        SetupInteractableTestScene(interactable);
        yield return InteractWithInteractable(this.brain, this.playerInstance, this.interactableBehaviour);
        Assert.IsTrue(interactable.HasInteracted);
    }

    [UnityTest]
    public IEnumerator DronesSpawnWithImprovisationUpgradeWorks()
    {
        DronesSpawnWithImprovisationUpgrade upgrade = ScriptableObject.CreateInstance<DronesSpawnWithImprovisationUpgrade>();

        var improvisation = ScriptableObject.CreateInstance<ScriptableBreakNoteImprovisation>();
        upgrade.Improvisation = improvisation;
        improvisation.TimesToDuplicate = ScriptableObject.CreateInstance<IntReference>();
        improvisation.TimesToDuplicate.Value = 1;

        var noteSelection = ScriptableObject.CreateInstance<ScriptableEveryNSelection>();
        noteSelection.N = improvisation.TimesToDuplicate;
        upgrade.Improvisation.NoteSelection = noteSelection;
        upgrade.Improvisation.BarSelection = noteSelection;

        SetupInteractableTestScene(upgrade);
        yield return InteractWithInteractable(
            this.brain, this.playerInstance, this.interactableBehaviour
        );
        yield return null;

        GameObject droneInstance = null;
        MessageRouter.RaiseMessage(new MsgSpawnDrone
        {
            Player = this.playerInstance,
            DroneType = MsgSpawnDrone.EDroneType.Melody,
            OnSpawned = (drone) =>
            {
                droneInstance = drone;
            }
        });

        while (droneInstance == null)
            yield return new WaitForSeconds(0.1f);

        var turntable = droneInstance.GetComponentInChildren<ITurntable>();
        Assert.NotZero(turntable.Improviser.NumberOfImprovisations);
    }

    [UnityTest]
    public IEnumerator BufferLevelNextLevelInteractableLoadsNextLevel()
    {
        BaseLevelConfiguration level = Resources.Load<BaseLevelConfiguration>("Levels/Level0");
        yield return LevelTests.LoadLevelAndWait(level);
        LevelTests.DisableEnemySpawner();
        var player = LevelTests.MakePlayerInvincible();

        NextLevelInteractable interactable = ScriptableObject.CreateInstance<NextLevelInteractable>();
        interactable.Interact(new Interactable.InteractArgs {
            Interactor = player
        });

        yield return new WaitForSeconds(1f);
        Assert.AreEqual(level.NextLevel, LevelLoader.CurrentLevelConfiguration);
    }

    [UnityTest]
    public IEnumerator InteractableGetsDeselected()
    {
        NullInteractable interactable = ScriptableObject.CreateInstance<NullInteractable>();
        SetupInteractableTestScene(interactable);
        yield return MoveTo(this.brain, this.playerInstance, interactableInstance.transform.position);
        Assert.IsTrue(interactableBehaviour.IsSelected);
        yield return MoveTo(this.brain, this.playerInstance, Vector3.zero);
        Assert.IsFalse(interactableBehaviour.IsSelected);
    }

    [UnityTest]
    public IEnumerator UpgradeDoesntGetPurchasedIfNotEnoughCurrency()
    {
        Upgrade upgrade = Resources.Load<Upgrade>("Upgrades/DronesSpawnWithBreakNote");
        SetupInteractableTestScene(upgrade);
        yield return MoveTo(this.brain, this.playerInstance, interactableInstance.transform.position);
        Assert.IsTrue(interactableBehaviour.IsSelected);
        brain.Shoot = true;
        yield return null;
        brain.Shoot = false;
        Assert.IsNotNull(interactableBehaviour);
        Assert.IsFalse(Upgrade.Upgrades.HasUpgrade(0, upgrade));
    }

    [UnityTest]
    public IEnumerator UpgradeGetsPurchasedIfEnoughCurrency()
    {
        Upgrade upgrade = Resources.Load<Upgrade>("Upgrades/DronesSpawnWithBreakNote");
        SetupInteractableTestScene(upgrade);
        yield return null;
        yield return null;
        SpawnResourcesToPayFor(this.playerInstance, upgrade);
        yield return MoveTo(this.brain, this.playerInstance, interactableInstance.transform.position);
        Assert.IsTrue(interactableBehaviour.IsSelected);
        brain.Shoot = true;
        yield return null;
        brain.Shoot = false;
        yield return null;
        Assert.IsTrue(interactableBehaviour == null);
        Assert.IsTrue(Upgrade.Upgrades.HasUpgrade(0, upgrade));
    }

    public static void SpawnResourcesToPayFor(
        GameObject playerInstance, 
        Upgrade upgrade
    ) {
        foreach (UpgradePriceCategory priceCategory in upgrade.Price)
        {
            for (int i = 0; i < priceCategory.Count; i++)
            {
                MessageRouter.RaiseMessage(new MsgSpawnDrone
                {
                    DroneType = priceCategory.DroneType,
                    Player = playerInstance
                });
            }
        }
    }

    public static TestCaseData[] GetPurchaseableUpgrades()
    {
        var upgrades = Resources.LoadAll<Upgrade>("Upgrades");
        return upgrades.Select(upgrade => new TestCaseData(upgrade).Returns(null)).ToArray();
    }

    [UnityTest, TestCaseSource(nameof(GetPurchaseableUpgrades))]
    public IEnumerator AllUpgradesCanBeBought(Upgrade upgrade)
    {
        SetupInteractableTestScene(upgrade);

        // Spawn currency needed.
        for (int i = 0; i < upgrade.Price.Count; i++)
        {
            foreach (UpgradePriceCategory priceCategory in upgrade.Price)
            {
                for (int j = 0; j < priceCategory.Count; j++)
                {
                    var targetType = priceCategory.DroneType;
                    if (targetType == MsgSpawnDrone.EDroneType.Any)
                        targetType = MsgSpawnDrone.EDroneType.Random;

                    MessageRouter.RaiseMessage(new MsgSpawnDrone
                    {
                        DroneType = priceCategory.DroneType,
                        Player = this.playerInstance
                    });
                }
            }
        }

        yield return MoveTo(this.brain, this.playerInstance, interactableInstance.transform.position);
        brain.Shoot = true;
        yield return null;
        brain.Shoot = false;

        yield return null;
        Assert.IsTrue(interactableBehaviour == null);
        Assert.IsTrue(Upgrade.Upgrades.HasUpgrade(0, upgrade));
    }

    [UnityTest]
    public IEnumerator PlayerRespawnsWithSamePermanentImprovisations()
    {
        Upgrade upgrade = Resources.Load<Upgrade>("Upgrades/PlayerSpawnWithBreakNote");
        SetupInteractableTestScene(upgrade);
        SpawnResourcesToPayFor(this.playerInstance, upgrade);
        yield return InteractWithInteractable(brain, playerInstance, interactableBehaviour);

        ImprovisationUpgrade improvisationUpgrade = upgrade as ImprovisationUpgrade;
        Improvisation improvisation = improvisationUpgrade.Improvisation.Get();

        MessageRouter.RaiseMessage(new MsgSpawnDrone
        {
            DroneType = EDroneType.Melody,
            Player = this.playerInstance
        });

        yield return null;

        this.playerInstance.GetComponent<Health>().Destroy();

        yield return new WaitForSeconds(2f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        TurntableBehaviour t = player.GetComponentInChildren<TurntableBehaviour>();

        Assert.AreEqual(1,t.Improviser.NumberOfImprovisations);
        Assert.AreEqual(improvisation,t.Improviser.GetEnumerable().ToList()[0]);
    }
}
