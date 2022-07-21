using UnityEngine;

namespace SpaceCabron
{
    // Also known as skull.
    public interface IBrainHolder {
        public IBrain Brain {get; set;}
    }

    public interface IBrain
    {
        public InputState GetInputState();
    }

    public class InputState
    {
        public Vector2 Movement { get; set; }
        public bool Shoot { get; set; }
    }

    public class Movement : MonoBehaviour, IBrainHolder
    {
        private IBrain Brain;
        public float Speed = 1f;

        IBrain IBrainHolder.Brain { 
            get => Brain; 
            set => Brain = value; 
        }

        void Update()
        {
            InputState state = Brain.GetInputState();
            transform.position += new Vector3(
                state.Movement.x, state.Movement.y, 0f
            )*Time.deltaTime*Speed;
        }
    }
}