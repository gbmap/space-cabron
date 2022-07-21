using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour
    {
        public Melody melody;
        private string _lastMelody;

        public int BPM = 60;
        public ScriptableCompositeBar Bar;
        public UnityEvent<OnNoteArgs> UnityEvent;

        [Header("Note Time")]
        public bool KeepNotePlaying;
        public float NoteTime = 0.1f;

        ITurntable _turntable;

        void Awake()
        {
            _turntable = new Turntable(BPM, melody, KeepNotePlaying, NoteTime);
        }

        void Update()
        {
            if (_lastMelody != melody.Notation)
                melody.Update();

            _turntable.BPM = BPM;
            _turntable.Update(OnNote);
        }

        void OnNote(OnNoteArgs note)
        {
            UnityEvent.Invoke(note);
        }
    }
}