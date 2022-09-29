using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCabron.UI {
    [ExecuteInEditMode]
    public class UISetMaterialIntProperty : MonoBehaviour
    {
        public Image image;
        public string property;
        public int value;

        void FixedUpdate() {
            image.material = new Material(image.material);
            image.material.SetInteger(property, value);
        }
    }
}