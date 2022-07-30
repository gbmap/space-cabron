using System;
using Gmap.ScriptableReferences;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour, ITurntable
    {
        public Melody melody;
        private string _lastMelody;

        // public int BPM = 60;
        public IntBusReference BPMReference;
        public ScriptableCompositeBar Bar;
        public UnityEvent<OnNoteArgs> UnityEvent;

        [Header("Note Time")]
        public bool KeepNotePlaying;
        public float NoteTime = 0.1f;

        ITurntable _turntable;
        public ITurntable Turntable
        {
            get 
            { 
                if (_turntable == null)
                    _turntable = new Turntable(BPMReference, melody, KeepNotePlaying, NoteTime, OnNote);
                return _turntable;
            }
        }

        public Improviser Improviser => Turntable.Improviser;

        public int BPM 
        { 
            get => BPMReference.Value; 
            set => BPMReference.Value = value; 
        }

        public Melody Melody => melody;
        public int NoteIndex => Turntable.NoteIndex;
        public int BarIndex => Turntable.BarIndex;

        void Awake()
        {
            BPMReference.Update();
        }

        void Update()
        {
            if (_lastMelody != melody.Notation)
            {
                melody = new Melody(melody.Notation);
                _lastMelody = melody.Notation;
            }

            Turntable.Update(OnNote);
        }

        public void SetMelody(Melody m)
        {
            melody = m;
            Turntable.SetMelody(m);
        }

        void OnNote(OnNoteArgs note)
        {
            UnityEvent.Invoke(note);
        }

        public void SetBPM(int bpm)
        {
            BPMReference.Value = bpm;
        }

        public void Update(Action<OnNoteArgs> OnNote) {}
    }
}