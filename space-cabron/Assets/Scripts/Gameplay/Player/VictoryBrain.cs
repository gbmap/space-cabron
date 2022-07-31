using System.Collections;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class VictoryBrain : MonoBehaviour, IBrain<InputState>
    {
        public System.Action OnAnimationEnded;

        InputState input = new InputState();
        IBrain<InputState> lastBrain;
        public InputState GetInputState()
        {
            return input;
        }

        void Start()
        {
            StartCoroutine(VictoryInputCoroutine());
        }

        IEnumerator VictoryInputCoroutine()
        {
            lastBrain = Brain<InputState>.Get(gameObject);

            MakeInvincible();
            input.Movement = Vector2.zero;
            yield return null;

            Vector3 centerPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 0.25f, 0f)
            );
            yield return MoveTowardsPosition(centerPosition);
            yield return new WaitForSeconds(3f);

            Vector3 outsideOfScreenTop = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 1.5f, 0f)
            );
            yield return MoveTowardsPosition(outsideOfScreenTop);
            InjectBrainToActor<InputState>.Inject(gameObject, lastBrain);
            DestroyImmediate(this);
            OnAnimationEnded?.Invoke();
        }

        private void MakeInvincible()
        {
            Health h = gameObject.GetComponent<Health>();
            if (h != null)
                h.CanTakeDamage = false;
        }

        IEnumerator MoveTowardsPosition(Vector3 targetPos)
        {
            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                input.Movement = Vector2.ClampMagnitude(targetPos - transform.position, 1f);
                yield return null;
            }
            input.Movement = Vector3.zero;
        }

        public static void Play(GameObject target, System.Action OnEnd=null)
        {
            VictoryBrain b = target.AddComponent<VictoryBrain>();
            if (OnEnd != null)
                b.OnAnimationEnded += OnEnd;
            InjectBrainToActor<InputState>.Inject(target, b);
        }
    }
}