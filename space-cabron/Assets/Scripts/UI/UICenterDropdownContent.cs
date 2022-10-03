using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpaceCabron.UI {
    public class UICenterDropdownContent : MonoBehaviour {
        [SerializeField] RectTransform content;
        public System.Int32 SelectedIndex;
        public float ItemHeight;

        void Update() {
            var rt = (transform as RectTransform);
            transform.localPosition = new Vector3(transform.localPosition.x, (SelectedIndex-1) * ItemHeight, 0);
            for (int i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                Selectable t = child.GetComponent<Selectable>();
                if (EventSystem.current.currentSelectedGameObject == t.gameObject) {
                    SelectedIndex = i;
                }
            }
        }

        public void UpdateSelectedItem(int index) {
            SelectedIndex = index;
        }
    }
}