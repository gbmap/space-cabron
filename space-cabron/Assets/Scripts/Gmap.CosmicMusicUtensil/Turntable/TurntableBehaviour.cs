using System;
using Gmap.ScriptableReferences;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.CosmicMusicUtensil
{
    public class TurntableSoftReference : ITurntable
    {
        public TurntableSoftReference(ITurntable turntable)
        {
            OriginalTurntable = turntable;
        }

        public ITurntable OriginalTurntable { get; set; }

        public int BPM { get => OriginalTurntable.BPM; set => OriginalTurntable.BPM = value; }
        public int MaxBPM { get => OriginalTurntable.MaxBPM; set => OriginalTurntable.MaxBPM = value; }
        public int NoteIndex => OriginalTurntable.NoteIndex;
        public int BarIndex => OriginalTurntable.BarIndex;
        public Melody Melody => OriginalTurntable.Melody;
        public Improviser Improviser => OriginalTurntable.Improviser;

        public Action<OnNoteArgs> OnNote { get => OriginalTurntable.OnNote; set => OriginalTurntable.OnNote = value; }
        public Action<OnBarArgs> OnBar { get => OriginalTurntable.OnBar; set => OriginalTurntable.OnBar = value; }
        public Action<OnImprovisationArgs> OnImprovisationAdded { get => OriginalTurntable.OnImprovisationAdded; set => OriginalTurntable.OnImprovisationAdded = value; }
        public Action<OnImprovisationArgs> OnImprovisationRemoved { get => OriginalTurntable.OnImprovisationRemoved; set => OriginalTurntable.OnImprovisationRemoved = value; }

        public void ApplyImprovisation(Improvisation improvisation, bool permanent)
        {
            OriginalTurntable.ApplyImprovisation(improvisation, permanent);
        }

        public void ApplyImprovisation(Improvisation improvisation, int time)
        {
            OriginalTurntable.ApplyImprovisation(improvisation, time);
        }

        public void SetImproviser(Improviser i)
        {
            OriginalTurntable.SetImproviser(i);
        }

        public void SetMelody(Melody m)
        {
            OriginalTurntable.SetMelody(m);
        }

        public void Update(Action<OnNoteArgs> OnNote)
        {
            OriginalTurntable.Update(OnNote);
        }
    }


    public class TurntableBehaviour : MonoBehaviour, ITurntable, IMelodyPlayer
    {
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
                    _turntable = new Turntable(BPMReference, new Melody(""), KeepNotePlaying, NoteTime);
                return _turntable;
            }
        }

        public void ForceSetTurntable(Turntable turntable)
        {
            _turntable = turntable;
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

        public int MelodyPlayerPriority => 1;

        void Awake()
        {
            if (BPMReference == null)
                return;

            BPMReference.Update();
        }

        void Start()
        {
            Turntable.OnBar += Callback_OnBar;
            Turntable.OnNote += Callback_OnNote;
        }

        void Update()
        {
            if (Turntable == null){
                return;
            }

            Turntable.Update(Callback_OnNote);
        }

        public void SetMelody(Melody m)
        {
            Turntable.SetMelody(m);
        }

        void Callback_OnNote(OnNoteArgs note)
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

        public void SetImproviser(Improviser i)
        {
            Turntable.SetImproviser(i);
        }

        public void Generate(MelodyFactory factory)
        {
            SetMelody(factory.GenerateMelody());
        }
    }
}