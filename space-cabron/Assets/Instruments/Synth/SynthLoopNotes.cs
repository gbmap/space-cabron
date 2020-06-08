using UnityEngine;

public class SynthLoopNotes : MonoBehaviour
{
    public ENote startingNote = ENote.C;
    public ENote currentNote;
    public int octave = 4;


    public NoteSequencer sequencer;
    public Synth synth;

    float lastHit;

    private void Awake()
    {
        currentNote = startingNote;
    }

    private void Update()
    {
        if (Time.time > lastHit + 1.0)
        {
            currentNote = startingNote;
            octave = 4;
        }
    }

    public void Play()
    {
        if (!synth)
            throw new System.Exception("No Synth configured!");

        var n = ENote.None;
        while (n == ENote.None)
        {
            n = (ENote)sequencer.NoteBag.Next();
        }
        synth.PlayKey(n, octave, 0.01);

        lastHit = Time.time;
    }
}
