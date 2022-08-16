using System;
using Gmap.ScriptableReferences;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableBehaviour : MonoBehaviour, ITurntable
    {
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
                    _turntable = new Turntable(BPMReference, new Melody(""), KeepNotePlaying, NoteTime, OnNote);
                return _turntable;
            }
        }

        public Improviser Improviser => Turntable.Improviser;

        public int BPM 
        { 
            get => Turntable.BPM; 
            set => Turntable.BPM = value; 
        }

        public Melody Melody => Turntable.Melody;
        public int NoteIndex => Turntable.NoteIndex;
        public int BarIndex => Turntable.BarIndex;

        void Awake()
        {
            BPMReference.Update();
        }

        void Update()
        {
            Turntable.Update(OnNote);
        }

        public void SetMelody(Melody m)
        {
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

        public void ApplyImprovisation(Improvisation improvisation, bool permanent)
        {
            Turntable.ApplyImprovisation(improvisation, permanent);
        }

    }
}