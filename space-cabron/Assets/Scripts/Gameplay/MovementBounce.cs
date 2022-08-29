using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class MovementBounce : MonoBehaviour
    {
        public float Speed = 10f;

        void Update()
        {
            transform.position += transform.up * Speed * Time.deltaTime;
            BounceOnScreenBorder();
        }

        private void BounceOnScreenBorder()
        {
            if (Camera.main == null)
                return;

            // Bounces on screen border
            var viewport = Camera.main.WorldToViewportPoint(transform.position);
            if (viewport.x > 1f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.up);
            }
            else if (viewport.y < 0f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.down);
            }
            else if (viewport.x < 0f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.right);
            }
            else if (viewport.x > 1f)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.left);
            }
        }
    }
}