using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap
{
    public class UpdateBackgroundShaderGlobalVariables : MonoBehaviour
    {
        public void OnNote(OnNoteArgs n)
        {
            Shader.SetGlobalFloat("_Beat", Time.time);
            Shader.SetGlobalFloat("_LastNoteDuration", n.Duration);
        }
    }
}