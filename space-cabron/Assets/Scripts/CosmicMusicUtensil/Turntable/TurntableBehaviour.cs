using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour
    {
        public int BPM = 60;
        public ScriptableCompositeBar Bar;
        public UnityEvent<OnNoteArgs> UnityEvent;

        ITurntable _turntable;

        void Awake()
        {
            _turntable = new Turntable(BPM, Bar);
        }

        void Update()
        {
            _turntable.Update(OnNote);
        }

        void OnNote(OnNoteArgs note)
        {
            UnityEvent.Invoke(note);
        }
    }
}