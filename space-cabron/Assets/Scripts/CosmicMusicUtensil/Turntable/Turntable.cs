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
        public int NoteIndex { get; }
        public int BarIndex { get; }
        public Melody Melody { get; }
        public Improviser Improviser { get; }
        void Update(System.Action<OnNoteArgs> OnNote);
        void SetMelody(Melody m);
        void ApplyImprovisation(Improvisation improvisation, bool permanent);
    }

    public class Turntable : ITurntable
    {
        public bool HoldNote;

        // public int BPM { get; set; }
        public IntBusReference BPMReference;
        public int BPM { get => BPMReference.Value; set => BPMReference.Value = value; }
        public float BPS
        {
            get { return BPMToBPS(BPM); }
        }

        int currentNoteIndex;
        public int NoteIndex => currentNoteIndex;

        int currentBarIndex;
        public int BarIndex => currentBarIndex;
        private Note LastNote { get; set; } 

        float noteTime = 0.1f;

        Melody melody;
        public Melody Melody { get { return melody; } }

        Improviser improviser = new Improviser();
        public Improviser Improviser { get { return improviser; } }


        public System.Action<OnNoteArgs> OnNote;

        Queue<Note> noteQueue = new Queue<Note>(50);

        public Turntable(IntBusReference bpmReference, Melody m, bool keepNotePlaying, float noteTime,
            System.Action<OnNoteArgs> onNote = null)
        {
            BPMReference = bpmReference;
            currentNoteIndex = 0;
            melody = m;
            HoldNote = keepNotePlaying;
            this.noteTime = noteTime;

            if (onNote != null)
                OnNote += onNote;
        }

        public void Update(System.Action<OnNoteArgs> OnNote)
        {
            if (Melody.IsEmpty)
                return;

            if (NoNotesToPlay())
                QueueNextNotes();
            PlayQueuedNotes();
        }

        void QueueNextNotes()
        {
            Note[] notesToPlay = improviser.Improvise(melody, currentBarIndex, melody.GetNote(currentNoteIndex), currentNoteIndex);
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
                HoldTime = Mathf.Max(0.1f, Mathf.Lerp(noteTime, noteTime*duration, HoldNote?1f:0f)),
                Duration = duration
            });
            LastNote = note;
        }

        public void SetMelody(Melody m)
        {
            melody = m;
        }

        private void AdvanceNoteIndex()
        {
            currentNoteIndex = (currentNoteIndex + 1) % melody.Length;
            if (currentNoteIndex == 0)
                currentBarIndex++;
        }

        public static float BPMToBPS(int bpm)
        {
            return (float)bpm/60f;
        }

        public void ApplyImprovisation(Improvisation improvisation, bool permanent)
        {
            if (permanent)
                Melody.ApplyImprovisation(improvisation);
            else
                Improviser.AddImprovisation(improvisation);
        }
    }
}