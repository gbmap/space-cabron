using System.Collections;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class VictoryBrain : MonoBehaviour, IBrain<InputState>
    {
        public System.Action OnAnimationEnded;

        InputState input;
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
            Health h = gameObject.GetComponent<Health>();
            if (h != null)
                h.CanTakeDamage = false;

            lastBrain = Brain<InputState>.Get(gameObject);
            input = new InputState
            {
                Movement = new Vector2(0f, 0f),
                Shoot = false
            };
            yield return null;

            Vector3 targetPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.25f, 0f));
            targetPos.z = 0f;
            while (Vector2.Distance(transform.position, targetPos) > 0.1f) {
                input.Movement = Vector2.ClampMagnitude(targetPos - transform.position, 1f);
                yield return null;
            }
            input.Movement = Vector3.zero;

            yield return new WaitForSeconds(3f);
            input.Movement = Vector2.up;
            while (Camera.main.WorldToViewportPoint(transform.position).y < 1.5f) {
                yield return null;
            }

            InjectBrainToActor<InputState>.Inject(gameObject, lastBrain);
            DestroyImmediate(this);
            OnAnimationEnded?.Invoke();
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