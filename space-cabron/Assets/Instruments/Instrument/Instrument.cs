using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[System.Serializable]
public class NoteChance
{
    public int Note;
    public int Weight;
}

public abstract class Instrument<T> : MonoBehaviour where T : System.Enum
{
    public BeatMaker BeatMaker;

    public List<NoteChance> NoteWeights = new List<NoteChance>();
    protected ShuffleBag<int> NoteBag = new ShuffleBag<int>();

    public System.Action<T> OnNote;

    public abstract int NoNote { get; }
    protected abstract T FromInt(int v);
    protected abstract void OnNoteCallback(T note);

    protected virtual void Awake()
    {
        NoteBag = GenerateNotes();
    }

    protected virtual void OnEnable()
    {
        BeatMaker.OnBeat += OnBeat;
        OnNote += OnNoteCallback;
    }

    protected virtual void OnDisable()
    {
        BeatMaker.OnBeat -= OnBeat;
        OnNote -= OnNoteCallback;
    }

    private void OnBeat()
    {
        int note = NoteBag.Next();
        if (note == NoNote) return;

        OnNote?.Invoke(FromInt(note));
    }

    public void UpdateNoteBag()
    {
        NoteBag = GenerateNotes();
    }

    protected ShuffleBag<int> GenerateNotes()
    {
        ShuffleBag<int> bag = new ShuffleBag<int>();
        foreach (NoteChance nc in NoteWeights)
        {
            bag.Add(nc.Note, nc.Weight);
        }
        return bag;
    }

}
