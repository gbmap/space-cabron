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
        public bool RandomizeInstrument { get; set; }
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
            if (Brain == null)
                return;

            InputState state = Brain.GetInputState(new InputStateArgs
            {
                Object = gameObject,
                Caller = this
            });

            LastPosition = transform.position;

            Vector3 velocity = new Vector3(
                state.Movement.x, state.Movement.y, 0f
            )*Time.deltaTime*Speed;

            if (Camera.main != null && Brain is ScriptableInputBrain) {
                Vector3 viewport = Camera.main.WorldToViewportPoint(transform.position + velocity);
                if (viewport.x < 0f || viewport.x > 1f)
                    velocity.x = 0f;
                else if (viewport.y < 0f || viewport.y > 1f)
                    velocity.y = 0f;
            }

            transform.position += velocity;

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