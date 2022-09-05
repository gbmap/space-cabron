using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class MovementBounce : MonoBehaviour
    {
        public float Speed = 10f;
        public float TimeBouncing = float.MaxValue;

        float startTime;
        bool shouldBounce = true;

        void Awake()
        {
            startTime = Time.time;
        }

        void Update()
        {
            transform.position += transform.up * Speed * Time.deltaTime;

            if (Time.time - startTime > TimeBouncing)
            {
                shouldBounce = false;
            }

            if (shouldBounce)
                BounceOnScreenBorder();
        }

        private void BounceOnScreenBorder()
        {
            if (Camera.main == null)
                return;

            // Bounces on screen border
            var viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPosition.x > 1f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.left);
            }
            else if (viewportPosition.x < 0f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.right);
            }
            else if (viewportPosition.y < 0f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.up);
            }
            else if (viewportPosition.y > 1f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.down);
            }
        }
    }
}