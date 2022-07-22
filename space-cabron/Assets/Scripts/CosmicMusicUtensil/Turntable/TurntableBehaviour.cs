using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour, ITurntable
    {
        public Melody Melody;
        private string _lastMelody;

        // public int BPM = 60;
        public IntBusReference BPMReference;
        public ScriptableCompositeBar Bar;
        public UnityEvent<OnNoteArgs> UnityEvent;

        [Header("Note Time")]
        public bool KeepNotePlaying;
        public float NoteTime = 0.1f;

        ITurntable _turntable;
        ITurntable Turntable
        {
            get 
            { 
                if (_turntable == null)
                    _turntable = new Turntable(BPMReference, Melody, KeepNotePlaying, NoteTime);
                return _turntable;
            }
        }

        public int BPM 
        { 
            get => BPMReference.Value; 
            set => BPMReference.Value = value; 
        }

        void Awake()
        {
            BPMReference.Update();
        }

        void Update()
        {
            if (_lastMelody != Melody.Notation)
                Melody.Update();

            Turntable.Update(OnNote);
        }

        public void SetMelody(Melody m)
        {
            Melody = m;
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