using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Z;

[System.Serializable]
public class NoteChance
{
    public int Note;
    public int Weight;
}

public abstract class NoteChances<T> : MonoBehaviour where T : System.Enum
{
    public ZBaseMarchPlayer Marcher;
    public ENote Root;

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
        UpdateNoteBag(Marcher.NotesInBar);
    }

    protected virtual void OnEnable()
    {
        Marcher.OnBeat += OnBeat;
        Marcher.OnNewBeat += OnNewBeat;
    }

    protected virtual void OnDisable()
    {
        Marcher.OnBeat -= OnBeat;
        Marcher.OnNewBeat -= OnNewBeat;
    }
    
    private void OnBeat(int b)
    {
        if (Notes.Length == 0) return;

        var notes = Notes[Marcher.CurrentBeat % Notes.Length];
        OnNoteCallback(notes.Select(n => FromInt(n)).ToArray());
    }

    private void OnNewBeat(int beatCount)
    {
        UpdateNoteBag(beatCount);
    }

    public void UpdateNoteBag(int beatCount, bool updateNotes = true)
    {
        NoteBag = GenerateNotes();

        if (updateNotes)
        {
            Notes = new int[beatCount][];
            for (int i = 0; i < Notes.Length; i++)
            {
                // sub notas, não confundir com uma sequência de notas dentro da bar
                Notes[i] = NoteBag.NextNoRepeat(UnityEngine.Random.Range(1, MaxNotesPerBeat), -1);
            }
        }
    }

    public void GenerateNewNotes()
    {
        Notes = new int[Marcher.NotesInBar][];
        for (int i = 0; i < Notes.Length; i++)
        {
            // sub notas, não confundir com uma sequência de notas dentro da bar
            Notes[i] = NoteBag.NextNoRepeat(UnityEngine.Random.Range(1, MaxNotesPerBeat), -1);
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

        UpdateNoteBag(Marcher.NotesInBar, false);
        Notes[UnityEngine.Random.Range(0, Notes.Length)][0] = n.Note;
    }

    public void ReplaceNote(int noteIndex, ENote newNote)
    {
        Notes[noteIndex][0] = (int)newNote;
    }

    public void StartWithRandomNote(bool addSilence = true)
    {
        int targetWeight = UnityEngine.Random.Range(0, NoteWeights.Count-1);
        Root = (ENote)targetWeight;

        var scale = Zapperz.GetScale(EScale.Major, Root);


        for (int i = 0; i < NoteWeights.Count; i++)
        {
            var nw = NoteWeights[i];
            if (targetWeight == i)
            {
                nw.Weight = UnityEngine.Random.Range(2, 4);
            }
            else if (nw.Note == (int)scale[1] ||
                nw.Note == (int)scale[2])
            {
                nw.Weight = 1;
            }
            else if (i == NoNote)
            {
                nw.Weight = System.Convert.ToInt32(addSilence);
            }
            else
            {
                nw.Weight = 0;
            }
        }

        UpdateNoteBag(Marcher.NotesInBar);
    }

    public void StartWithNotes(ENote[] notes, bool addSilence = true)
    {

    }
}
