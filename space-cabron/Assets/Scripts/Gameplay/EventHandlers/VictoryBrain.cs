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
            SetCanTakeDamage(false);
            input.Movement = Vector2.zero;
            yield return AnimationCoroutine();
            SetCanTakeDamage(true);

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
            StopAllCoroutines();
            FinishAnimationAndDestroy();
        }

        public static void Play<T>(GameObject target, System.Action OnEnd=null) where T : AnimationBrain
        {
            AnimationBrain[] brains = target.GetComponents<AnimationBrain>();
            System.Array.ForEach(brains, b=> b.CancelAnimation());

            T b = target.AddComponent<T>();
            if (OnEnd != null)
                b.OnAnimationEnded += OnEnd;
            InjectBrainToActor<InputState>.Inject(target, b);
        }
    }

    public class VictoryBrain : AnimationBrain
    {
        protected override IEnumerator AnimationCoroutine()
        {
            Vector3 centerPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 0.25f, 0f)
            );
            yield return MoveTowardsPosition(centerPosition);
            yield return new WaitForSeconds(3f);

            Vector3 outsideOfScreenTop = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 1.5f, 0f)
            );
            yield return MoveTowardsPosition(outsideOfScreenTop);
        }
    }
}