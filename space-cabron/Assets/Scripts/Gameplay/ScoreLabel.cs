using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class ScoreLabel : MonoBehaviour
    {
        public int Score
        {
            set
            {
                Text.text = value.ToString();
            }
        }

        public float Duration = 1.25f;
        [SerializeField] TMPro.TextMeshPro Text;

        float startTime;

        void Start()
        {
            startTime = Time.time;
        }

        void Update()
        {
            float t = GetAnimationTime();
            Text.alpha = 1.0f - t;

            if (t >= 1f)
                Destroy(gameObject);
        }

        float GetAnimationTime()
        {
            return Mathf.Clamp01((Time.time-startTime)/Duration);
        }
    }
}