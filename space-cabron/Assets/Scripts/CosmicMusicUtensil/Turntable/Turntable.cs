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
        public bool HoldNote;

        // public int BPM { get; set; }
        public IntBusReference BPMReference;
        public int BPM { get => BPMReference.Value; set => BPMReference.Value = value; }
        public float BPS
        {
            get { return (60f/(float)BPMReference.Value); }
        }
        
        int   currentNoteIndex;
        int   currentBarIndex;
        public Note LastNote { get; private set; } 
        public Note CurrentNote { get; private set; }

        float _lastPlayedNote;
        float _noteTime = 0.1f;

        Melody Melody;
        Improviser improviser = new Improviser();
        Queue<Note> noteQueue = new Queue<Note>(50);

        public System.Action<OnNoteArgs> OnNote;

        public Turntable(IntBusReference bpmReference, Melody m, bool keepNotePlaying, float noteTime,
            System.Action<OnNoteArgs> onNote = null)
        {
            BPMReference = bpmReference;
            currentNoteIndex = 0;
            Melody = m;
            HoldNote = keepNotePlaying;
            _noteTime = noteTime;

            if (onNote != null)
                OnNote += onNote;
        }

        public void Update(System.Action<OnNoteArgs> OnNote)
        {
            if (NoNotesToPlay())
                QueueNextNotes();
            PlayQueuedNotes();
        }

        void QueueNextNotes()
        {
            Note[] notesToPlay = improviser.Improvise(Melody, currentBarIndex, Melody.GetNote(currentNoteIndex), currentNoteIndex);
            System.Array.ForEach(notesToPlay, n => noteQueue.Enqueue(n));
            AdvanceNoteIndex();
        }

        void PlayQueuedNotes()
        {
            if (NoNotesToPlay() || !LastNoteFinishedPlaying())
                return;

            PlayNote(noteQueue.Dequeue());
        }

        private bool LastNoteFinishedPlaying()
        {
            return LastNote == null 
                || Time.time % LastNote.GetDuration(BPS) <= Time.deltaTime;
        }

        private bool NoNotesToPlay()
        {
            return noteQueue.Count == 0;
        }

        private void PlayNote(Note note)
        {
            float duration = note.GetDuration(BPS);
            OnNote?.Invoke(new OnNoteArgs
            {
                Note = note,
                HoldTime = Mathf.Lerp(_noteTime, _noteTime*duration, Convert.ToSingle(HoldNote)),
                Duration = duration
            });
            LastNote = note;
        }

        public void SetMelody(Melody m)
        {
            Melody = m;
        }

        private void AdvanceNoteIndex()
        {
            currentNoteIndex = (currentNoteIndex + 1) % Melody.Length;
        }
    }
}