using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Gmap.CosmicMusicUtensil
{
    public class HelmProxy : MonoBehaviour
    {
        public AudioHelm.HelmController Controller;
        public AudioHelm.Sampler Sampler;

        public bool RandomizePatchOnLoad;
        public float ValueRange = 0.1f;
        public float ProbabilityOfChange = 0.5f;

        public int Channel 
        {
            get => Controller.channel;
            set => Controller.channel = value;
        }

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
            if (RandomizePatchOnLoad)
                RandomizePatch(helmPatch);

            Controller.LoadPatch(helmPatch);
        }

        public void RandomizePatch(AudioHelm.HelmPatch patch)
        {
            var config = patch.patchData;
            var fields = config.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(float))
                {
                    var value = (float)field.GetValue(config);
                    var newValue = value + Random.Range(-ValueRange, ValueRange);
                    field.SetValue(config, newValue);
                }
            }
        }
    }

}