using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour
    {
        public Melody Melody;
        private string _lastMelody;

        public int BPM = 60;
        public ScriptableCompositeBar Bar;
        public UnityEvent<OnNoteArgs> UnityEvent;

        [Header("Note Time")]
        public bool KeepNotePlaying;
        public float NoteTime = 0.1f;

        ITurntable _turntable;

        void Start()
        {
            _turntable = new Turntable(BPM, Melody, KeepNotePlaying, NoteTime);
        }

        void Update()
        {
            if (_lastMelody != Melody.Notation)
                Melody.Update();

            _turntable.BPM = BPM;
            _turntable.Update(OnNote);
        }

        public void SetMelody(Melody m)
        {
            Melody = m;
            _turntable.SetMelody(m);
        }

        void OnNote(OnNoteArgs note)
        {
            UnityEvent.Invoke(note);
        }
    }
}