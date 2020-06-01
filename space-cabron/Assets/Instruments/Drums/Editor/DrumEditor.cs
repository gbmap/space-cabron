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
            E.PropertyField(serializedObject.FindProperty("source"));
            E.PropertyField(serializedObject.FindProperty("instrument"));

            E.Separator();
            E.Space();

            BeatMakerEditorUtility.DrawInspector(ref d.BPM, ref d.MaxSubBeats, ref d.MaxInstrumentsPerBeat, ref d.NBeats);

            if (sampleChances = E.Foldout(sampleChances, "Sample Chances"))
            {
                if (d.SampleWeights == null || d.SampleWeights.Count == 0) d.GenerateNewWeights();

                foreach (EInstrumentAudio sample in System.Enum.GetValues(typeof(EInstrumentAudio)))
                {
                    d.SampleWeights[(int)sample].Weight = E.IntSlider(sample.ToString(), d.SampleWeights[(int)sample].Weight, 0, 10);
                }
            }

            if (GUILayout.Button("Generate New Pattern"))
            {
                d.GenerateNewPattern(true);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }


}