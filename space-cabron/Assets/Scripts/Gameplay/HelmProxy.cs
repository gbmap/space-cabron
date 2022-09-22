using System.Collections;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class HelmProxy : MonoBehaviour
    {
        public AudioHelm.HelmController Controller;
        public AudioHelm.Sampler Sampler;

        public bool RandomizePatchOnLoad;
        public bool LoadRandomPatchOnRandomize;
        public HelmSynthGenerator.EGeneratorProfile Profile;

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
            if (gameObject == null) {
                return;
            }

            UnityEngine.Debug.Log($"Loading patch: {patch.name} in {transform.parent}/{gameObject.name}");
            StartCoroutine(LoadPatchCoroutine(patch));
        }

        private IEnumerator LoadPatchCoroutine(TextAsset patch)
        {
            yield return new WaitForSeconds(0.1f);
            AudioHelm.HelmPatch helmPatch = GetPatch();
            helmPatch.LoadPatchDataFromText(patch.text);

            Controller = GetComponent<AudioHelm.HelmController>();
            if (RandomizePatchOnLoad)
                Randomize();

            Controller.LoadPatch(helmPatch);
        }

        private AudioHelm.HelmPatch GetPatch()
        {
            var helmPatch = gameObject.GetComponent<AudioHelm.HelmPatch>();
            if (!helmPatch)
                helmPatch = gameObject.AddComponent<AudioHelm.HelmPatch>();
            return helmPatch;
        }

        public void Randomize() {
            if (!RandomizePatchOnLoad) {
                return ;
            }

            // return;
            var patch = GetPatch();

            if (patch.patchData == null)
                patch.patchData = new AudioHelm.HelmPatchFormat();
            patch.patchData.settings = HelmSynthGenerator.GeneratorFactory
                                                         .Create(Profile, true)
                                                         .Generate(patch.patchData.settings);

            Debug.Log(HelmSynthGenerator.HelmSynthGenerator.HelmSettingsToString(patch.patchData.settings));
            Controller.LoadPatch(patch);
        }
    }

}