using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class ConstantRotation : MonoBehaviour
    {
        public float AngularSpeed;

        // Update is called once per frame
        void Update()
        {
            transform.rotation = Quaternion.Euler(
                0f, 
                0f, 
                transform.rotation.eulerAngles.z + AngularSpeed * Time.deltaTime
            );
        }
    }
}