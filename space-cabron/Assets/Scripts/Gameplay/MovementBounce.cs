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
            // Bounces on screen border
            var screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            if (transform.position.y > screenBounds.y)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.up);
            }
            else if (transform.position.y < -screenBounds.y)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.down);
            }
            else if (transform.position.x > screenBounds.x)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.right);
            }
            else if (transform.position.x < -screenBounds.x)
            {
                transform.up = Vector3.Reflect(transform.up, Vector3.left);
            }
        }
    }
}