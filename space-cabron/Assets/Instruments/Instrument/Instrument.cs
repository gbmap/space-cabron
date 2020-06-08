using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [HideInInspector]
    public List<NoteChance> NoteWeights = new List<NoteChance>();
    public ShuffleBag<int> NoteBag = new ShuffleBag<int>();

    [Range(1, 4)]
    public int MaxNotesPerBeat;

    protected int[][] Notes;

    public System.Action<T[]> OnNote;

    public abstract int NoNote { get; }
    protected abstract T FromInt(int v);
    protected abstract int ToInt(T v);
    protected abstract void OnNoteCallback(T[] note);

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        UpdateNoteBag();
    }

    protected virtual void OnEnable()
    {
        BeatMaker.OnBeat += OnBeat;
        BeatMaker.OnNewBeat += OnNewBeat;
        OnNote += OnNoteCallback;
    }

    protected virtual void OnDisable()
    {
        BeatMaker.OnBeat -= OnBeat;
        BeatMaker.OnNewBeat -= OnNewBeat;
        OnNote -= OnNoteCallback;
    }
    
    private void OnBeat()
    {
        var notes = Notes[BeatMaker.CurrentBeat];
        //if (note == NoNote) return;
        OnNote?.Invoke(notes.Select(n => FromInt(n)).ToArray());
    }

    private void OnNewBeat(int beatCount)
    {
        UpdateNoteBag();
    }

    public void UpdateNoteBag(bool updateNotes = true)
    {
        NoteBag = GenerateNotes();

        if (updateNotes)
        {
            Notes = new int[BeatMaker.Loop.BeatCount][];
            for (int i = 0; i < Notes.Length; i++)
            {
                Notes[i] = NoteBag.NextNoRepeat(UnityEngine.Random.Range(1, MaxNotesPerBeat), -1);
            }
        }
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

    public void AddNote(T note)
    {
        var n = NoteWeights.Find(nw => nw.Note == ToInt(note));
        n.Weight++;

        UpdateNoteBag(false);
        Notes[UnityEngine.Random.Range(0, Notes.Length)][0] = n.Note;
    }

    public void StartWithRandomNote()
    {
        int targetWeight = UnityEngine.Random.Range(0, NoteWeights.Count-1);
        for (int i = 0; i < NoteWeights.Count; i++)
        {
            var nw = NoteWeights[i];
            if (targetWeight == i)
            {
                nw.Weight = UnityEngine.Random.Range(2, 4);
            }
            else if (i == NoNote)
            {
                nw.Weight = 1;
            }
            else
            {
                nw.Weight = 0;
            }
        }

        UpdateNoteBag();
    }
}
