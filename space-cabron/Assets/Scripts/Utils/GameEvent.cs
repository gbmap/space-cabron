using System.Collections.Generic;
using UnityEngine;

namespace Gmap.Utils
{
    public interface IGameEventListener
    {
        void OnEventRaised();
    }

    /// <summary>
    /// https://www.youtube.com/watch?v=raQ3iHhE_Kk&t=1867s
    /// </summary>
    [CreateAssetMenu(menuName="Gmap/Utils/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private List<IGameEventListener> m_listeners =
            new List<IGameEventListener>();

        public void Raise()
        {
            for (int i = m_listeners.Count-1; i >= 0; i--) 
                m_listeners[i].OnEventRaised();

        }

        public void RegisterListener(IGameEventListener listener)
        {
            m_listeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener listener)
        {
            m_listeners.Remove(listener);
        }
    }
}