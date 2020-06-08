using UnityEngine;

/*
 * 
 * MELODY GENERATOR
 * Algorithms to generate melodies. Basically a ZMelody factory.
 * 
 * */

public enum EZapperzMelodyType
{
    SkipAndWalk
}

[System.Serializable]
public class ZMelody
{
    public ENote[] notes;
    public int[] octaves;

    public int Length
    {
        get { return notes.Length; }
    }
}

public class ZapperzMellowMelodies
{
    /*
     * Melody generator similar to old dungeon generators,
     * It departs from the root note, walks towards the left or the right,
     * or skips a note left or right inside a scale, and ends on the root.
     * 
     * */
    public static ZMelody GenerateSkipAndWalkMelody(ENote root, EScale scale, int sOctave, int nNotes)
    {
        ENote[] notes = new ENote[nNotes];
        int[] octaves = new int[nNotes];
        ENote[] scaleNotes = Zapperz.GetScale(scale, root);

        ENote n = root; // current note
        int o = sOctave; // current octave
        int position = 0; // position of the walker
        int i = 0; // which note is being generated
        while (i < nNotes)
        {
            notes[i] = n;
            octaves[i] = o;
            int direction = (int)Mathf.Sign(Random.value - 0.5f);
            int step = Mathf.CeilToInt(Random.value * 2f) * direction;
            
            if (position + step >= scaleNotes.Length)
            {
                o++;
            }

            position = (position + step) % scaleNotes.Length;
            if (position < 0) // position is negative, wrap around octave
            {
                o--;
                position = scaleNotes.Length + position;
            }

            n = scaleNotes[position];
            i++;
        }

        return new ZMelody {
            notes = notes,
            octaves = octaves
        };
    }
}
