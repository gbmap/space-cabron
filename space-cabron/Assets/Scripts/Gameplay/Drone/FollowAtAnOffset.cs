using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class FollowAtAnOffset : MonoBehaviour
    {
        public Vector3 Offset;
        public Transform Target;

        void Update()
        {
            if (Target == null)
                return;

            transform.position = Vector3.Lerp(transform.position, Target.position + Offset, Time.deltaTime);
        }
    }
}