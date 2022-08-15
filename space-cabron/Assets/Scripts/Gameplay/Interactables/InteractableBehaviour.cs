using Gmap.Gameplay;
using UnityEngine;
using UnityEngine.Events;

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
        public TMPro.TextMeshPro Description;

        public UnityEvent OnIsSelected;
        public UnityEvent OnIsDeselected;
        public UnityEvent OnInteract;

        bool isSelected;
        public bool IsSelected 
        { 
            get => isSelected; 
            private set 
            {
                isSelected = value;
                if (isSelected)
                    OnIsSelected.Invoke();
                else
                    OnIsDeselected.Invoke();
            }
        }


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
                bool success = Interactable.Interact(new Interactable.InteractArgs
                {
                    Interactor = interactor.obj
                });

                if (success)
                    Destroy(gameObject);
            }
        }

        public void Configure(Interactable upgrade)
        {
            this.Interactable = upgrade;
            if (Description != null)
                Description.text = upgrade.Description;
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
            if (otherBrainHolder.Brain == interactor.brain)
            {
                interactor = null;
                IsSelected = false;
            }
        }
    }
}