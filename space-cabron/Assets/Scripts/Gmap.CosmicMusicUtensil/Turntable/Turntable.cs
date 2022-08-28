using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class OnNoteArgs
    {
        public Turntable Turntable { get; set; }
        public Note Note { get; set; }
        public float Duration { get; set; }
        public float HoldTime { get; set; } = 0.1f;
    }

    public class OnBarArgs : OnNoteArgs {}

    public class OnImprovisationArgs
    {
        public ITurntable Turntable { get; set; }
        public Improvisation Improvisation { get; set; }
        public int Life { get; set; }
    }

    public interface ITurntable
    {
        int BPM { get; set; }
        int MaxBPM { get; set; }
        public int NoteIndex { get; }
        public int BarIndex { get; }
        public Melody Melody { get; }
        public Improviser Improviser { get; }
        void Update(System.Action<OnNoteArgs> OnNote);
        void SetMelody(Melody m);
        void ApplyImprovisation(Improvisation improvisation, bool permanent);
        void ApplyImprovisation(Improvisation improvisation, int time);
        public System.Action<OnNoteArgs> OnNote { get; set; }
        public System.Action<OnBarArgs> OnBar { get; set; }
        public System.Action<OnImprovisationArgs> OnImprovisationAdded { get; set; }
        public System.Action<OnImprovisationArgs> OnImprovisationRemoved { get; set; }
    }

    public class Turntable : ITurntable
    {
        public bool HoldNote;

        // public int BPM { get; set; }
        public IntBusReference BPMReference;
        public int BPM 
        { 
            get => BPMReference.Value; 
            set => BPMReference.Value = Mathf.Min(MaxBPM, value); 
        }

        public float BPS
        {
            get { return BPMToBPS(BPM); }
        }

        int currentNoteIndex;
        public int NoteIndex => currentNoteIndex;

        int currentBarIndex;
        public int BarIndex => currentBarIndex;
        public Note LastNote { get; private set; } 
        public float LastNoteTime { get; private set; }

        float noteTime = 0.1f;

        Melody melody;
        public Melody Melody { get { return melody; } }

        Improviser improviser; 
        public Improviser Improviser { get { return improviser; } }

        public Action<OnNoteArgs> OnNote { get; set; }
        public Action<OnBarArgs> OnBar { get; set; }
        public Action<OnImprovisationArgs> OnImprovisationAdded { get; set; }
        public Action<OnImprovisationArgs> OnImprovisationRemoved { get; set; }

        int maxBPM;
        public int MaxBPM 
        { 
            get { return maxBPM; } 
            set { maxBPM = value; }
        }


        Queue<Note> noteQueue = new Queue<Note>(50);

        public Turntable(
            IntBusReference bpmReference, 
            Melody m, 
            bool keepNotePlaying, 
            float noteTime,
            System.Action<OnNoteArgs> onNote = null
        ) {
            BPMReference = bpmReference;
            MaxBPM = 120;
            currentNoteIndex = 0;
            melody = m;
            HoldNote = keepNotePlaying;
            improviser = new Improviser(this, OnImprovisationRemoved);
            this.noteTime = noteTime;

            if (onNote != null)
                OnNote += onNote;
        }

        public void Update(System.Action<OnNoteArgs> OnNote)
        {
            if (Melody.IsEmpty)
                return;

            PlayQueuedNotes();
            if (NoNotesToPlay())
            {
                AdvanceNoteIndex();
                QueueNextNotes();
            }  
        }

        void QueueNextNotes()
        {
            Note[] notesToPlay = improviser.Improvise(melody, currentBarIndex, melody.GetNote(currentNoteIndex), currentNoteIndex);
            System.Array.ForEach(notesToPlay, n => noteQueue.Enqueue(n));
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
                || (Time.time - LastNoteTime) >= LastNote.GetDuration(BPS);
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
                Turntable = this,
                Note = note,
                HoldTime = Mathf.Max(0.1f, Mathf.Lerp(noteTime, noteTime*duration, HoldNote?1f:0f)),
                Duration = duration
            });
            LastNote = note;
            LastNoteTime = Time.time;
        }

        public void SetMelody(Melody m)
        {
            melody = m;
        }

        private void AdvanceNoteIndex()
        {
            currentNoteIndex = (currentNoteIndex + 1) % melody.Length;
            if (currentNoteIndex == 0)
            {
                currentBarIndex++;
                OnBar?.Invoke(new OnBarArgs
                {
                    Turntable = this,
                    Note = melody.GetNote(0)
                });
            }
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
            {
                OnImprovisationAdded?.Invoke(new OnImprovisationArgs
                {
                    Turntable = this,
                    Improvisation = improvisation,
                    Life = -1
                });
                Improviser.AddImprovisation(improvisation);
            }
        }

        public void ApplyImprovisation(Improvisation improvisation, int time)
        {
            OnImprovisationAdded?.Invoke(new OnImprovisationArgs 
            {
                Improvisation = improvisation,
                Life = time,
                Turntable = this
            });
            improviser.AddImprovisation(improvisation, time);
        }

        #if UNITY_EDITOR
        public string DebugString()
        {
            Note[] notes = noteQueue.ToArray();
            if (notes.Length == 0)
                return "";
            if (LastNote == null)
                return "";
            string notesStr = notes.Select(n=>n.AsString()).Aggregate((a,b)=>a+";"+b);
            string s = $"LastNote: {LastNote.AsString()} \nNoteQueue: " + notesStr;
            return s;
        }
        #endif
    }
}