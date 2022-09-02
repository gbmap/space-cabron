using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class InputState
    {
        public Vector2 Movement { get; set; }
        public bool Shoot { get; set; }
        public bool Pause { get; set; }
        public EColor Color { get; set; }
    }

    public class Movement : MonoBehaviour, IBrainHolder<InputState>
    {
        public bool ScaleTowardsDirection;

        private IBrain<InputState> Brain;
        public float Speed = 1f;

        public Vector3 LastPosition { get; private set; }

        private Animator animator;
        private int horizontalHash = Animator.StringToHash("X");

        IBrain<InputState> IBrainHolder<InputState>.Brain { 
            get => Brain; 
            set => Brain = value; 
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            InputState state = Brain.GetInputState(new InputStateArgs
            {
                Object = gameObject,
                Caller = this
            });

            LastPosition = transform.position;
            transform.position += new Vector3(
                state.Movement.x, state.Movement.y, 0f
            )*Time.deltaTime*Speed;

            if (animator)
                animator.SetFloat(horizontalHash, state.Movement.x);

            if (ScaleTowardsDirection)
            {
                Vector3 direction = (transform.position - LastPosition).normalized;
                transform.localScale = new Vector3(
                    transform.localScale.x * -Mathf.Sign(direction.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }
    }
}