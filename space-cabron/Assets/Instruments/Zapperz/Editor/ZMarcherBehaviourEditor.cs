using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Z
{
    [CustomEditor(typeof(ZMarchPlayer))]
    public class ZMarcherBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ZMarchPlayer.BPM = EditorGUILayout.IntSlider(ZMarchPlayer.BPM, 30, 320);

            DrawDefaultInspector();
            /*
            if (GUILayout.Button("Generate New Wave"))
            {
                (target as ZapperzCurtisMarcher).GenerateNewBeat();
            }
            */
        }
    }
}