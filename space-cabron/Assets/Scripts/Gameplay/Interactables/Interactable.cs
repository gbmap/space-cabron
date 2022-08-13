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

        public abstract void Interact(InteractArgs args);
    }

    public class NullInteractable : Interactable
    {
        public bool HasInteracted { get; private set; }
        public override void Interact(InteractArgs args)
        {
            HasInteracted = true;
        }
    }
}