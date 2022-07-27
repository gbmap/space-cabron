using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Debug
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class DebugTurntableInfo : MonoBehaviour
    {
        ITurntable turntableBehaviour;

        void Awake() 
        {
            turntableBehaviour = GetComponent<ITurntable>();
        }


        void OnGUI()
        {
            GUILayout.Label($"Note Index: {turntableBehaviour.NoteIndex}");
            GUILayout.Label($"Bar Index: {turntableBehaviour.BarIndex}");
        }
    }
}