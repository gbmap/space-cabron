using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class InputState
    {
        public Vector2 Movement { get; set; }
        public bool Shoot { get; set; }
        public bool Pause { get; set; }
    }

    public class Movement : MonoBehaviour, IBrainHolder<InputState>
    {
        private IBrain<InputState> Brain;
        public float Speed = 1f;

        IBrain<InputState> IBrainHolder<InputState>.Brain { 
            get => Brain; 
            set => Brain = value; 
        }

        void Update()
        {
            InputState state = Brain.GetInputState(new InputStateArgs
            {
                Object = gameObject
            });
            transform.position += new Vector3(
                state.Movement.x, state.Movement.y, 0f
            )*Time.deltaTime*Speed;
        }
    }
}