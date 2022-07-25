using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class HelmProxy : MonoBehaviour
    {
        public AudioHelm.HelmController Controller;
        public AudioHelm.Sampler Sampler;

        public void Play(OnNoteArgs args)
        {
            var note = args.Note;
            if (Controller)
                Controller.NoteOn(Note.ToMIDI(note.Tone, note.Octave), Random.value, args.HoldTime);
            else if (Sampler)
                Sampler.NoteOn(Note.ToMIDI(note.Tone, note.Octave), Random.value);
        }

    }

}