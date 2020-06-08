using UnityEngine;

public class SynthLoopNotes : MonoBehaviour
{
    public ENote startingNote = ENote.C;
    public ENote currentNote;
    public int octave = 4;


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

        synth.PlayKey(currentNote, octave, 0.01);

        currentNote++;
        if (currentNote == ENote.None)
        {
            currentNote = ENote.A;
        }
        else if (currentNote == ENote.B)
        {
            octave++;
        }

        lastHit = Time.time;
    }
}
