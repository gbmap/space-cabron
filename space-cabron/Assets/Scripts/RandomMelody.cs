using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.CosmicMusicUtensil;

public class RandomMelody : MonoBehaviour
{
    public int BPM = 60;
    public int Octave = 4;
    public Gmap.CosmicMusicUtensil.Scale Scale;
    public Gmap.CosmicMusicUtensil.ENote Note;
    public AudioHelm.HelmController Controller;

    // public BarIntervals Bar;
    private int m_barCursor = 0;

    public Melody Melody;
    private int m_melodyCursor = 0;

    void OnEnable()
    {
        if (!Application.isPlaying)
            return; 


        // StartCoroutine(Play());
    }

    /*
    IEnumerator Play()
    {
        while (enabled)
        {
            var note = Melody.GetNote(Note, Scale, m_melodyCursor++);
            float timeToNextNote = Bar.GetNoteTime(BPM, m_barCursor++);

            if (note != Gmap.CosmicMusicUtensil.ENote.None)
                Controller.NoteOn(Gmap.CosmicMusicUtensil.Note.ToMIDI(note, Octave), 0.5f, timeToNextNote);
            yield return new WaitForSeconds(timeToNextNote);
        }

    }
    */
}
