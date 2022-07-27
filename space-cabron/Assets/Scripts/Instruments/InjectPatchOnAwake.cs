using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Instruments
{
    public abstract class Injectable : MonoBehaviour {}

    [RequireComponent(typeof(AudioHelm.HelmController))]
    public class InjectPatchOnAwake : Injectable
    {
        public TextAsset Patch;
        AudioHelm.HelmController controller;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            var helmPatch = gameObject.GetComponent<AudioHelm.HelmPatch>();
            if (!helmPatch)
                helmPatch = gameObject.AddComponent<AudioHelm.HelmPatch>();
            helmPatch.LoadPatchDataFromText(Patch.text);

            controller = GetComponent<AudioHelm.HelmController>();
            controller.LoadPatch(helmPatch);
        }
    }
}