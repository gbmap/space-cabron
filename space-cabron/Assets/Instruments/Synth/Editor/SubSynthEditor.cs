using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SC
{
    [CustomEditor(typeof(SubSynth))]
    public class SubSynthEditor : SynthEditor
    {
        public override void OnInspectorGUI()
        {
            var ss = target as SubSynth;

            ss.TargetNote = (ENote)EditorGUILayout.EnumPopup("Target Note", ss.TargetNote);
            ss.OverrideNote = (ENote)EditorGUILayout.EnumPopup("Override Note", ss.OverrideNote);

            base.OnInspectorGUI();
        }
    }
}