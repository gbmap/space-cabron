using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpaceCabron.UI
{
    public class BlinkObject : MonoBehaviour
    {
        public GameObject TargetObject;
        public bool Loop;
        public float TimeVisible = 1f;
        public float Time = 3f;
        public bool EndInactive = false;
        public bool PlayOnAwake = false;

        public void Play()
        {
            StartCoroutine(BlinkCoroutine());
        }

        private IEnumerator BlinkCoroutine()
        {
            yield break;
        }
    }
}