using System.Collections;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class VictoryBrain : MonoBehaviour, IBrain<InputState>
    {
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
            IBrain<InputState> brain = Brain<InputState>.Get(gameObject);
            input = new InputState
            {
                Movement = new Vector2(0f, 0f),
                Shoot = false
            };
            yield return null;

            Vector3 targetPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.75f, 0f));
            targetPos.z = 0f;
            while (Vector2.Distance(transform.position, targetPos) > 0.1f) {
                input.Movement = Vector2.ClampMagnitude(targetPos - transform.position, 1f);
                yield return null;
            }
            input.Movement = Vector3.zero;

            yield return new WaitForSeconds(3f);
            input.Movement = Vector2.up;
            yield return new WaitForSeconds(5f);

            InjectBrainToActor<InputState>.Inject(gameObject, lastBrain);
        }

        public static void Play(GameObject target)
        {
            VictoryBrain b = target.AddComponent<VictoryBrain>();
            InjectBrainToActor<InputState>.Inject(target, b);
        }
    }
}