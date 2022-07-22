using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{

    public class OnNoteArgs
    {
        public Note Note { get; set; }
        public float Duration { get; set; }
        public float HoldTime { get; set; } = 0.1f;
    }

    public interface ITurntable
    {
        int BPM { get; set; }
        void Update(System.Action<OnNoteArgs> OnNote);
        void SetMelody(Melody m);
    }

    public class Turntable : ITurntable
    {
        public Melody Melody;
        public bool HoldNote;

        // public int BPM { get; set; }
        public IntBusReference BPMReference;
        public int BPM { get => BPMReference.Value; set => BPMReference.Value = value; }
        public float BPS
        {
            get { return (60f/(float)BPMReference.Value); }
        }
        
        int   _noteIndex;
        public Note LastNote { get { return Melody.GetNote(_noteIndex-1); } }
        public Note CurrentNote { get { return Melody.GetNote(_noteIndex); } }

        float _lastPlayedNote;
        float _noteTime = 0.1f;

        public Turntable(IntBusReference bpmReference, Melody m, bool keepNotePlaying, float noteTime)
        {
            BPMReference = bpmReference;
            _noteIndex = 0;
            Melody = m;
            HoldNote = keepNotePlaying;
            _noteTime = noteTime;
        }

        public void Update(System.Action<OnNoteArgs> OnNote)
        {
            float lastNoteDuration = LastNote.GetTime(BPS);
            // bool shouldPlay = (Time.time - _lastPlayedNote) >= lastNoteDuration;
            bool shouldPlay = (Time.time % lastNoteDuration) <= Time.deltaTime; /// lastNoteDuration

            if (!shouldPlay)
                return;

            OnNote?.Invoke(new OnNoteArgs
            {
                Note = CurrentNote,
                HoldTime = Mathf.Lerp(_noteTime, _noteTime*lastNoteDuration, Convert.ToSingle(HoldNote)),
                Duration = lastNoteDuration
            });

            _noteIndex++;
            _lastPlayedNote = Time.time;
        }

        public void SetMelody(Melody m)
        {
            Melody = m;
        }
    }
}