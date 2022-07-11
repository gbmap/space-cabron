using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface IModifiableBar
    {
        void AddNote(Note n);
        void RemoveNote(int i);
        void InsertAt(Note n, int i);
    }

    public interface IBar
    {
        Note GetNote(int i);
        float GetTotalTime(int bpm);

        bool IsValid { get; }
        int Length { get; }
    }

    [System.Serializable]
    public class Bar : IBar, IModifiableBar
    {
        public Vector2Int TimeSignature = new Vector2Int(4,4);
        public List<Note> Notes = new List<Note>();

        public int Length => Notes.Count;
        public bool IsValid => Mathf.Approximately(Notes.Sum(n => 1f/n.Interval), 1f);

        public Bar() {}
        public Bar(List<Note> notes)
        {
            Notes = notes;
        }

        public void AddNote(Note n)
        {
            Notes.Add(n);
        }

        // C# sucks.
        public static int MathMod(int a, int b) {
            return (Mathf.Abs(a * b) + a) % b;
        }

        public Note GetNote(int i)
        {
            i = MathMod(i, Notes.Count);
            return Notes[i];
        }

        public float GetTotalTime(int bpm)
        {
            float bps = ((float)bpm)/60;
            return 1f/bps * ((float)TimeSignature.x)/TimeSignature.y;
        }

        public void InsertAt(Note n, int i)
        {
            Notes.Insert(i, n);
        }

        public void RemoveNote(int i)
        {
            Notes.RemoveAt(i);
        }

        public static IBar Create()
        {
            return Create(new Vector2Int(4,4));
        }

        public static IBar Create(Vector2Int timeSignature)
        {
            // Bar bar = ScriptableObject.CreateInstance<Bar>();
            Bar bar = new Bar();
            bar.TimeSignature = timeSignature;
            return bar;
        }
    }
}