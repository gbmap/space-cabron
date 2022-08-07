using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class MoveTowardsObjectWhenClose : MonoBehaviour
    {
        public LayerMask Layer;
        public float Distance = 5f;
        public float Acceleration = 1f;

        private Collider2D target;
        private Vector3 velocity;
        private Coroutine coroutineCheckObject;

        void Start()
        {
            coroutineCheckObject = StartCoroutine(CheckObject());
        }

        void FixedUpdate()
        {
            if (target == null)
            {
                if (coroutineCheckObject == null)
                    coroutineCheckObject = StartCoroutine(CheckObject());
                return;
            }

            Vector3 delta = target.transform.position - transform.position; 
            velocity += delta.normalized * (1f/(delta.sqrMagnitude*2f))
                     * Acceleration * Time.fixedDeltaTime;
            transform.position += velocity;
        }

        IEnumerator CheckObject()
        {
            while (target == null)
            {
                Collider2D obj = Physics2D.OverlapCircle(
                    transform.position, 
                    Distance, 
                    Layer.value,
                    0f
                );
                if (obj != null)
                {
                    target = obj;
                    continue;
                }
                yield return new WaitForSeconds(0.2f);
            }
            coroutineCheckObject = null;
        }
        

    }
}