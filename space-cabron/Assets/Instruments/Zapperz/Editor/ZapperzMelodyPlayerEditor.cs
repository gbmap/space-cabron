using UnityEditor;
using UnityEngine;

namespace Z
{
    [CustomEditor(typeof(ZMelodyPlayerSynth))]
    public class ZMelodyPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var zmp = target as ZMelodyPlayerSynth;

            if (zmp.Melody.Length != 0)
            {
                EditorGUILayout.LabelField(zmp.CurrentNote.ToString());
            }

            /*
            if (GUILayout.Button("Generate New Melody"))
            {
                (target as ZMelodyPlayer).GenerateMelody();
            }
            */
        }
    }
}