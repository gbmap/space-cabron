using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron
{
    public class UpdateBackgroundShaderGlobalVariables : MonoBehaviour
    {
        public void OnBeat(OnNoteArgs n)
        {
            Shader.SetGlobalFloat("_Beat", Time.time);
            Shader.SetGlobalFloat("_LastNoteDuration", n.Duration);
        }
    }
}