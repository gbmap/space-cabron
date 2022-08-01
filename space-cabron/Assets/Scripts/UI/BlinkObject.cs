using System.Collections;
using UnityEngine;

namespace Gmap.UI
{
    public class BlinkObject : MonoBehaviour
    {
        public GameObject TargetObject;
        public bool Loop;
        public float TimeVisible = 1f;
        public float Time = 3f;
        public bool PlayOnAwake = false;

        void OnEnable()
        {
            if (PlayOnAwake)
                Play();
        }

        public void Play()
        {
            StartCoroutine(BlinkCoroutine());
        }

        private IEnumerator BlinkCoroutine()
        {
            while (true)
            {
                TargetObject.SetActive(true);
                yield return new WaitForSecondsRealtime(TimeVisible);
                TargetObject.SetActive(false);
                yield return new WaitForSecondsRealtime(TimeVisible);
            }
        }
    }
}