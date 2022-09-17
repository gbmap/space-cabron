using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.Instruments
{
    public abstract class Injectable : MonoBehaviour {}

    [RequireComponent(typeof(AudioHelm.HelmController))]
    public class InjectPatchOnAwake : Injectable
    {
        public TextAssetPool PatchPool;
        public TextAsset SpecificPatch;
        AudioHelm.HelmController controller;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);
            HelmProxy proxy = GetComponent<HelmProxy>();
            // var helmPatch = gameObject.GetComponent<AudioHelm.HelmPatch>();
            // if (!helmPatch)
            //     helmPatch = gameObject.AddComponent<AudioHelm.HelmPatch>();

            if (PatchPool)
                SpecificPatch = PatchPool.GetNext();

            proxy.LoadPatch(SpecificPatch);
            
            // helmPatch.LoadPatchDataFromText(SpecificPatch.text);
            

            // controller = GetComponent<AudioHelm.HelmController>();
            // controller.LoadPatch(helmPatch);
        }
    }
}