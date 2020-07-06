using UnityEngine;
using Z;

public class SynthLoopNotes : MonoBehaviour
{
    public EScale scale;
    public ENote note;

    public int octave = 4;
    public Synth synth;

    ZMelody melody;

    float lastHit;
    private int currentNote;

    private void Awake()
    {
        melody = ZMelodyGenerator.GenerateSkipAndWalkMelody(note, scale, octave, 5);
    }

    private void Update()
    {
        if (Time.time > lastHit + 1.0)
        {
            //currentNote = 0;
            //octave = 4;
        }
    }

    public void Play()
    {
        if (!synth)
            throw new System.Exception("No Synth configured!");

        var n = melody.notes[currentNote];
        synth.PlayKey(n, octave, 0.01);

        currentNote = (currentNote + 1) % melody.notes.Length;
        lastHit = Time.time;
    }
}
