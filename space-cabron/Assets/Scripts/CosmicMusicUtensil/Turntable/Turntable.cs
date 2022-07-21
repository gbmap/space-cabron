using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{

    public class OnNoteArgs
    {
        public Note Note { get; set; }
    }

    public interface ITurntable
    {
        int BPM {get; set;}
        IBar Bar {get;}

        void Update(System.Action<OnNoteArgs> OnNote);
    }

    public class Turntable : ITurntable
    {
        IBar _bar;
        public IBar Bar => _bar;

        public int BPM { get; set; }
        public float BPS
        {
            get { return ((float)BPM)/60f; }
        }
        
        int   _noteIndex;
        public Note LastNote { get { return Bar.GetNote(_noteIndex-1); } }
        public Note CurrentNote { get { return Bar.GetNote(_noteIndex); } }

        float _lastPlayedNote;

        public Turntable(int BPM, IBar bar)
        {
            this.BPM = BPM;
            _bar = bar;
            _noteIndex = 0;
        }

        public void Update(System.Action<OnNoteArgs> OnNote)
        {
            float lastNoteDuration = LastNote.GetTime(BPS);
            bool shouldPlay = (Time.time - _lastPlayedNote) >= lastNoteDuration;

            if (!shouldPlay)
                return;

            OnNote?.Invoke(new OnNoteArgs
            {
                Note = CurrentNote
            });

            _noteIndex++;
            _lastPlayedNote = Time.time;
        }
    }
}