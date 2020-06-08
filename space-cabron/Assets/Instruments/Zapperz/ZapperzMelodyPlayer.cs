using System;
using UnityEngine;

public class ZapperzMelodyPlayer : MonoBehaviour
{
    [Header("References")]
    //public BeatMaker beatMaker;
    public ZapperzCurtisMarcher marcher;
    public Synth synth;

    [Header("Melody Configuration")]
    public EZapperzMelodyType MelodyType;

    public ENote rootNote;
    public EScale scale;
    public int octave = 3;
    public int numberOfNotes = 8;

    [Header("Notes Created")]
    public ZMelody notes;

    [Header("Current Note")]
    public ENote currentNote;
    public int currentOctave;
    public int currentNoteIndex;


    private void Awake()
    {
        GenerateMelody();
    }

    private void OnEnable()
    {
        marcher.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        marcher.OnBeat -= OnBeat;   
    }

    private void OnBeat(int beatType)
    {
        synth.PlayKey(currentNote, currentOctave);
        currentNoteIndex = (currentNoteIndex + 1) % notes.Length;
        currentNote = notes.notes[currentNoteIndex];
        currentOctave = notes.octaves[currentNoteIndex];
    }

    public void GenerateMelody()
    {
        notes = ZapperzMellowMelodies.GenerateSkipAndWalkMelody(rootNote, scale, 3, numberOfNotes);
        currentNoteIndex = 0;
        currentNote = notes.notes[0];
        currentOctave = notes.octaves[0];
    }
}
