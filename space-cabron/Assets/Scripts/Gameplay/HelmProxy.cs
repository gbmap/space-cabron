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
                Controller.NoteOn(Note.ToMIDI(note.Tone, note.Octave), Random.Range(0.5f, 1f), args.HoldTime);
            else if (Sampler)
                Sampler.NoteOn(Note.ToMIDI(note.Tone, 3), 1f);
        }

        public void LoadPatch(TextAsset patch)
        {
            UnityEngine.Debug.Log($"Loading patch: {patch.name} in {transform.parent}/{gameObject.name}");
            StartCoroutine(LoadPatchCoroutine(patch));
        }

        private IEnumerator LoadPatchCoroutine(TextAsset patch)
        {
            yield return new WaitForSeconds(0.1f);
            var helmPatch = gameObject.GetComponent<AudioHelm.HelmPatch>();
            if (!helmPatch)
                helmPatch = gameObject.AddComponent<AudioHelm.HelmPatch>();
            helmPatch.LoadPatchDataFromText(patch.text);

            Controller = GetComponent<AudioHelm.HelmController>();
            Controller.LoadPatch(helmPatch);
        }
    }

}