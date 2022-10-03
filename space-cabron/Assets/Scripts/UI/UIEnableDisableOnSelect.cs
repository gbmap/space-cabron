using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceCabron.UI
{
    public class UIEnableDisableOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Behaviour behaviour;

        void OnDisable() {
            behaviour.enabled = false;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            behaviour.enabled = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            behaviour.enabled = true;
        }
    }
}