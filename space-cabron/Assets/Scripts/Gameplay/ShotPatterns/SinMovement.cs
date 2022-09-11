using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class SinMovement : MonoBehaviour
    {
        public float Speed = 1f;
        public float Frequency = 1f;
        public float Amplitude = 1f;
        public float TimeOffset = 0f;

        private float InitialAngle = 0f;
        private float timer = 0f;

        public bool UseCos;

        void OnEnable()
        {
            InitialAngle = transform.localEulerAngles.z;
            timer = 0f;
        }

        void Update()
        {
            transform.position += transform.up * Speed * Time.deltaTime;

            float x = timer * Frequency;
            float z = 0f;
            if (UseCos)
                z = Mathf.Cos(x+TimeOffset);
            else
                z = Mathf.Sin(x+TimeOffset);

            transform.localRotation = Quaternion.Euler(
                0f, 
                0f,
                InitialAngle + z * Amplitude
            );

            timer += Time.deltaTime;
        }
    }
}