using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SpaceCabron.UI
{
    public class UIHandleCancelEvent : MonoBehaviour, ICancelHandler {
        public UnityEvent OnCancel;

        void ICancelHandler.OnCancel(BaseEventData eventData)
        {
            OnCancel.Invoke();
        }
    }
}