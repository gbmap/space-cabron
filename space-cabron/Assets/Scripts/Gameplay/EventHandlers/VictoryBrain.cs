using System.Collections;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public abstract class AnimationBrain : MonoBehaviour, IBrain<InputState>
    {
        public System.Action OnAnimationEnded;

        protected InputState input = new InputState();
        protected IBrain<InputState> initialBrain;
        public InputState GetInputState(InputStateArgs args)
        {
            return input;
        }

        void Awake()
        {
            initialBrain = Brain<InputState>.Get(gameObject);
        }

        void Start()
        {
            StartCoroutine(Coroutine());
        }

        IEnumerator Coroutine()
        {
            bool canTakeDamage = GetCanTakeDamage();
            SetCanTakeDamage(false);
            input.Movement = Vector2.zero;
            yield return AnimationCoroutine();
            SetCanTakeDamage(canTakeDamage);

            FinishAnimationAndDestroy();
        }

        private void FinishAnimationAndDestroy()
        {
            InjectBrainToActor<InputState>.Inject(gameObject, initialBrain);
            OnAnimationEnded?.Invoke();
            Destroy(this);
        }

        protected abstract IEnumerator AnimationCoroutine();

        protected void SetCanTakeDamage(bool v)
        {
            Health h = gameObject.GetComponent<Health>();
            if (h != null)
                h.CanTakeDamage = v;
        }

        protected bool GetCanTakeDamage() {
            Health h = gameObject.GetComponent<Health>();
            if (h != null)
                return h.CanTakeDamage;
            return false;
        }

        protected IEnumerator MoveTowardsPosition(Vector3 targetPos)
        {
            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                input.Movement = Vector2.ClampMagnitude(targetPos - transform.position, 1f);
                yield return null;
            }
            input.Movement = Vector3.zero;
        }

        public void CancelAnimation()
        {
            Debug.Log($"[AnimationBrain] Cancelling animation {GetType().Name}");
            StopAllCoroutines();
            FinishAnimationAndDestroy();
        }

        public static T Play<T>(GameObject target, System.Action OnEnd=null) where T : AnimationBrain
        {
            AnimationBrain[] brains = target.GetComponents<AnimationBrain>();
            System.Array.ForEach(brains, b=> b.CancelAnimation());

            T component = target.AddComponent<T>();
            if (OnEnd != null)
                component.OnAnimationEnded += OnEnd;
            InjectBrainToActor<InputState>.Inject(target, component);

            return component;
        }
    }

    public class VictoryBrain : AnimationBrain
    {
        public int Index = 0;
        public int MaxPlayers = 2;

        protected override IEnumerator AnimationCoroutine()
        {
            float halfWidth = 0.25f;

            float x = Mathf.Lerp(0.5f-halfWidth, 0.5f+halfWidth, ((float)Index)/MaxPlayers);
            Vector3 centerPosition = Vector3.zero;
            if (Camera.main != null)
                centerPosition = Camera.main.ViewportToWorldPoint(
                    new Vector3(x, 0.25f, 0f)
                );
            yield return MoveTowardsPosition(centerPosition);
            yield return new WaitForSeconds(3f);

            Vector3 outsideOfScreenTop = Vector3.up * 15f + Vector3.right * centerPosition.x;
            yield return MoveTowardsPosition(outsideOfScreenTop);
        }
    }
}