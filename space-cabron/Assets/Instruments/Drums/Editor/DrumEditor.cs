using UnityEditor;
using UnityEngine;

namespace SC
{
    using E = EditorGUILayout;

    [CustomEditor(typeof(Drum))]
    public class DrumEditor : Editor
    {
        bool sampleChances = false;

        public override void OnInspectorGUI()
        {
            Drum d = target as Drum;

            E.LabelField("References");
            E.PropertyField(serializedObject.FindProperty("BeatMaker"));
            E.PropertyField(serializedObject.FindProperty("Source"));
            E.PropertyField(serializedObject.FindProperty("Instrument"));

            E.Separator();
            E.Space();

            if (sampleChances = E.Foldout(sampleChances, "Sample Chances"))
            {
                if (InstrumentEditor<EInstrumentAudio>.DrawChancesInspector(ref d.NoteWeights))
                {
                    EditorUtility.SetDirty(target);
                }
            }

            if (GUILayout.Button("Generate New Pattern"))
            {
                d.UpdateNoteBag();
            }


            serializedObject.ApplyModifiedProperties();
        }
    }


}