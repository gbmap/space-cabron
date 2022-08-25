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

        private float InitialAngle = 0f;
        private float InitialTime = 0f;
        private float timer = 0f;

        public bool UseCos;

        void Start()
        {
            InitialAngle = transform.localEulerAngles.z;
        }

        void OnEnable()
        {
            timer = 0f;
            // InitialAngle = transform.localEulerAngles.z;
        }

        void Update()
        {
            transform.position += transform.up * Speed * Time.deltaTime;

            float x = timer * Frequency;
            float z = 0f;
            if (UseCos)
                z = Mathf.Cos(x);
            else
                z = Mathf.Sin(x);

            transform.localRotation = Quaternion.Euler(
                0f, 
                0f,
                InitialAngle + z * Amplitude
            );

            timer += Time.deltaTime;
        }
    }
}