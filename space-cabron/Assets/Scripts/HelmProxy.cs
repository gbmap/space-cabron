using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class HelmProxy : MonoBehaviour
    {
        public AudioHelm.HelmController Controller;

        public void Play(OnNoteArgs args)
        {
            var note = args.Note;
            Controller.NoteOn(Note.ToMIDI(note.Tone, 4), 0.5f, args.Time);
        }

    }

}