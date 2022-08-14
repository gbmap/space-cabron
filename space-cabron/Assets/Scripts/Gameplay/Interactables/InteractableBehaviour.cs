using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay.Interactables
{
    public class InteractableBehaviour : MonoBehaviour 
    {
        protected class Interactor
        {
            public GameObject obj;
            public IBrain<InputState> brain;
        }

        public string Tag = "Player";
        public Interactable Interactable;

        public bool IsSelected { get; private set; }

        private Interactor interactor;

        void Awake()
        {
            if (Interactable != null)
                Configure(Interactable);
        }

        void Update()
        {
            if (interactor == null)
                return;

            var inputState = new InputStateArgs
            {
                Object = interactor.obj,
                Caller = this
            };
            if (interactor.brain.GetInputState(inputState).Shoot)
            {
                Interactable.Interact(new Interactable.InteractArgs
                {
                    Interactor = interactor.obj
                });
                Destroy(gameObject);
            }
        }

        public void Configure(Interactable upgrade)
        {
            this.Interactable = upgrade;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag(Tag) && !IsSelected)
                return;

            interactor = new Interactor
            {
                brain = collider.GetComponent<IBrainHolder<InputState>>().Brain,
                obj = collider.gameObject
            };
            IsSelected = true;
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if (!collider.CompareTag(Tag))
                return;

            IBrainHolder<InputState> otherBrainHolder = collider.GetComponent<IBrainHolder<InputState>>();
            if (otherBrainHolder == interactor.brain)
            {
                interactor = null;
                IsSelected = false;
            }
        }
    }
}