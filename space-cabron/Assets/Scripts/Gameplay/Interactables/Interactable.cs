using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    public abstract class Interactable : ScriptableObject 
    {
        public class InteractArgs
        {
            public GameObject Interactor;
        }

        public string Description;
        public abstract Sprite Icon { get; }

        public abstract bool Interact(InteractArgs args);

        public static GameObject CreateInteractable(Interactable interactable)
        {
            return CreateInteractable(interactable, Vector3.zero);
        }

        public static GameObject CreateInteractable(
            Interactable interactable,
            Vector3 position
        ) {
            GameObject interactablePrefab = Resources.Load<GameObject>("Interactable");
            var interactableInstance = Instantiate(
                interactablePrefab,
                position,
                Quaternion.identity
            );
            MakeInteractable(interactableInstance, interactable);
            return interactableInstance;
        }

        public static void MakeInteractable(GameObject obj, Interactable interactable)
        {
            var interactableBehaviour = obj.GetComponent<InteractableBehaviour>();
            if (interactableBehaviour == null)
                throw new System.Exception("Object {obj} must have an InteractableBehaviour component.");
            interactableBehaviour.Configure(interactable);
        }
    }

    public class NullInteractable : Interactable
    {
        public bool HasInteracted { get; private set; }

        public override Sprite Icon => null;

        public override bool Interact(InteractArgs args)
        {
            HasInteracted = true;
            return true;
        }
    }
}