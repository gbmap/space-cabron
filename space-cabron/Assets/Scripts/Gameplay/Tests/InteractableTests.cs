using System.Collections;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using NUnit.Framework;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Interactables;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.TestTools;

public class InteractableTests 
{
    public class PlayerControlBrain : ScriptableAIBrain<InputState>
    {
        public Vector2 Movement = new Vector2();
        public bool Shoot = false;
        public override InputState GetInputState(InputStateArgs args)
        {
            return new InputState
            {
                Movement = this.Movement,
                Shoot = this.Shoot
            };
        }
    }

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

    private IEnumerator GetInteractable(Interactable interactable)
    {
        var eventHandlerPrefab = Resources.Load<GameObject>("EventHandlers");
        this.eventHandlers = GameObject.Instantiate(eventHandlerPrefab);

        var playerPrefab = Resources.Load("Player");
        this.playerInstance = GameObject.Instantiate(
            playerPrefab, 
            Vector3.zero, 
            Quaternion.identity
        ) as GameObject;

        brain = ScriptableObject.CreateInstance<PlayerControlBrain>();
        InjectBrainToActor<InputState>.Inject(playerInstance, brain);

        var interactablePrefab = Resources.Load("Interactable");
        GameObject interactableInstance = GameObject.Instantiate(
            interactablePrefab, 
            new Vector3(0, 1, 0), 
            Quaternion.identity
        ) as GameObject;

        InteractableBehaviour ib = interactableInstance.GetComponent<InteractableBehaviour>();
        ib.Interactable = interactable;

        for (int i = 0; i < 10; i++)
        {
            Assert.IsFalse(ib.IsSelected);
            yield return new WaitForSeconds(0.1f);
        }

        while (playerInstance.transform.position.y < 1f)
        {
            brain.Movement = Vector2.up;
            yield return null;
        }
        brain.Movement = Vector2.zero;

        yield return null;
        Assert.IsTrue(ib.IsSelected);
        yield return null;
        brain.Shoot = true;
        yield return null;
    }

    [UnityTest]
    public IEnumerator InteractableBehaviourRunsInteractOnScriptableObject()
    {
        NullInteractable interactable = ScriptableObject.CreateInstance<NullInteractable>();
        yield return GetInteractable(interactable);
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

        yield return GetInteractable(upgrade);

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
}
