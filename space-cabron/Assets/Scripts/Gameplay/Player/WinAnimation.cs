using System.Collections;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class VictoryBrain : MonoBehaviour, IBrain<InputState>
    {
        InputState input;
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
            input = new InputState
            {
                Movement = new Vector2(0f, 0f),
                Shoot = false
            };
            yield return null;

            // while (Vector3.Distance(transform.position, ))

        

            
        }
    }

    public class WinAnimation : MonoBehaviour
    {
        public void PlayWinAnimation(GameObject player)
        {

        }

        private IEnumerator PlayWinAnimation()
        {
            yield break;

        }
    }
}