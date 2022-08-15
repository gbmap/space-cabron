using System.Collections;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using NUnit.Framework;
using SpaceCabron.Gameplay.Interactables;
using SpaceCabron.Gameplay.Level;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.TestTools;
using static LevelTests;

public class InteractableTests 
{
    [TearDown]
    public void TearDown()
    {
        MessageRouter.Reset();
        GameObject.Destroy(playerInstance);
        GameObject.Destroy(eventHandlers);

        var turntables = GameObject.FindObjectsOfType<TurntableBehaviour>();
        System.Array.ForEach(turntables, t => GameObject.Destroy(t.gameObject));

        var interactables = GameObject.FindObjectsOfType<InteractableBehaviour>();
        System.Array.ForEach(interactables, t => GameObject.Destroy(t.gameObject));
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

        this.brain = InjectPlayerWithControlBrain(this.playerInstance);

        interactableInstance = Interactable.CreateInteractable(
            interactable, new Vector3(0, 1, 0)
        );
        interactableBehaviour = interactableInstance.GetComponent<InteractableBehaviour>();
    }
    
    IEnumerator InteractWithInteractable()
    {
        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(interactableBehaviour.IsSelected);
            yield return new WaitForSeconds(0.1f);
        }

        yield return MoveTo(interactableInstance.transform.position);
        yield return null;
        Assert.IsTrue(interactableBehaviour.IsSelected);
        yield return null;
        brain.Shoot = true;
        yield return null;
    }

    IEnumerator MoveTo(Vector3 targetPosition)
    {
        while (Vector3.Distance(playerInstance.transform.position, targetPosition) > 0.01f)
        {
            brain.Movement = Vector3.ClampMagnitude(
                targetPosition - playerInstance.transform.position,
                1
            );
            yield return null;
        }
        brain.Movement = Vector2.zero;
    }

    [UnityTest]
    public IEnumerator InteractableBehaviourRunsInteractOnScriptableObject()
    {
        NullInteractable interactable = ScriptableObject.CreateInstance<NullInteractable>();
        SetupInteractableTestScene(interactable);
        yield return InteractWithInteractable();
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
        yield return InteractWithInteractable();
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
        yield return MoveTo(interactableInstance.transform.position);
        Assert.IsTrue(interactableBehaviour.IsSelected);
        yield return MoveTo(Vector3.zero);
        Assert.IsFalse(interactableBehaviour.IsSelected);
    }

    [UnityTest]
    public IEnumerator UpgradeDoesntGetPurchasedIfNotEnoughCurrency()
    {
        Upgrade upgrade = Resources.Load<Upgrade>("Upgrades/DronesSpawnWithBreakNote");
        SetupInteractableTestScene(upgrade);
        yield return MoveTo(interactableInstance.transform.position);
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
        foreach (UpgradePriceCategory priceCategory in upgrade.Price)
        {
            for (int i = 0; i < priceCategory.Count; i++)
            {
                MessageRouter.RaiseMessage(new MsgSpawnDrone
                {
                    DroneType = priceCategory.DroneType,
                    Player = this.playerInstance
                });
            }
        }
        yield return MoveTo(interactableInstance.transform.position);
        Assert.IsTrue(interactableBehaviour.IsSelected);
        brain.Shoot = true;
        yield return null;
        brain.Shoot = false;
        yield return null;
        Assert.IsTrue(interactableBehaviour == null);
        Assert.IsTrue(Upgrade.Upgrades.HasUpgrade(0, upgrade));
    }
}
