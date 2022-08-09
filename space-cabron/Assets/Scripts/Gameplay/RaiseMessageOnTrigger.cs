using System.Collections;
using System.Collections.Generic;
using Gmap.Utils;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class RaiseMessageOnTrigger : MonoBehaviour
    {
        public SpaceCabronMessageReference MessageReference;

        void OnTriggerEnter2D(Collider2D other)
        {
            MessageReference.Raise();
        }
    }
}