using UnityEngine;
using UnityEngine.Events;

namespace Gmap.Utils
{
    public class GameEventListener : MonoBehaviour, IGameEventListener
    {
        public GameEvent GameEvent;
        public UnityEvent Event;

        void OnEnable()
        {
            GameEvent.RegisterListener(this);
        }

        void OnDisable()
        {
            GameEvent.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Event.Invoke();
        }
    }
}