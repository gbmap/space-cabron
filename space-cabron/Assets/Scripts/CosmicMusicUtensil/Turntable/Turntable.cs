using System;
using System.Collections;
using System.Collections.Generic;
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
        int BPM {get; set;}
        void Update(System.Action<OnNoteArgs> OnNote);
        void SetMelody(Melody m);
    }

    public class Turntable : ITurntable
    {
        public Melody Melody;

        public int BPM { get; set; }
        public float BPS
        {
            get { return (60f/(float)BPM); }
        }
        
        int   _noteIndex;
        public Note LastNote { get { return Melody.GetNote(_noteIndex-1); } }
        public Note CurrentNote { get { return Melody.GetNote(_noteIndex); } }

        public bool KeepNotePlaying;

        float _lastPlayedNote;
        float _noteTime = 0.1f;

        public Turntable(int BPM, Melody m, bool keepNotePlaying, float noteTime)
        {
            this.BPM = BPM;
            _noteIndex = 0;
            KeepNotePlaying = keepNotePlaying;
            Melody = m;
            _noteTime = noteTime;
        }

        public void Update(System.Action<OnNoteArgs> OnNote)
        {
            float lastNoteDuration = LastNote.GetTime(BPS);
            bool shouldPlay = (Time.time - _lastPlayedNote) >= lastNoteDuration;

            if (!shouldPlay)
                return;

            OnNote?.Invoke(new OnNoteArgs
            {
                Note = CurrentNote,
                HoldTime = Mathf.Min(_noteTime*lastNoteDuration, 0.1f),
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