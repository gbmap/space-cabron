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
        public UnityEvent<OnBarArgs> UnityEventBar;

        [Header("Note Time")]
        public bool KeepNotePlaying;
        public float NoteTime = 0.1f;

        ITurntable _turntable;
        public ITurntable Turntable
        {
            get 
            { 
                if (_turntable == null)
                    _turntable = new Turntable(BPMReference, new Melody(""), KeepNotePlaying, NoteTime, OnNotePlayed);
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

        public Action<OnNoteArgs> OnNote 
        {
            get { return Turntable.OnNote; }
            set { Turntable.OnNote = value; }
        }

        public Action<OnBarArgs> OnBar
        {
            get => Turntable.OnBar;
            set => Turntable.OnBar = value;
        }

        public Action<OnImprovisationArgs> OnImprovisationAdded
        {
            get { return Turntable.OnImprovisationAdded; }
            set { Turntable.OnImprovisationAdded = value; }
        }

        public Action<OnImprovisationArgs> OnImprovisationRemoved
        {
            get { return Turntable.OnImprovisationRemoved; }
            set { Turntable.OnImprovisationRemoved = value; }
        }

        public int MaxBPM
        {
            get { return Turntable.MaxBPM; }
            set { Turntable.MaxBPM = value; }
        }

        void Awake()
        {
            BPMReference.Update();
        }

        void Start()
        {
            Turntable.OnBar += Callback_OnBar;
        }

        void Update()
        {
            Turntable.Update(OnNotePlayed);
        }

        public void SetMelody(Melody m)
        {
            Turntable.SetMelody(m);
        }

        void OnNotePlayed(OnNoteArgs note)
        {
            UnityEvent.Invoke(note);
        }

        void Callback_OnBar(OnBarArgs bar)
        {
            UnityEventBar.Invoke(bar);
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

        public void ApplyImprovisation(Improvisation improvisation, int time)
        {
            Turntable.ApplyImprovisation(improvisation, time);
        }

        void OnDestroy()
        {
            this.UnityEvent.RemoveAllListeners();
        }
    }
}