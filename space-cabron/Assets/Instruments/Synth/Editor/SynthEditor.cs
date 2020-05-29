using UnityEditor;
using UnityEngine;


namespace SC {

    using E = EditorGUILayout;

    [CustomEditor(typeof(Synth))]
    public class SynthEditor : Editor
    {
        bool foldout = false;

        public override void OnInspectorGUI()
        {
            var s = target as Synth;

            E.PropertyField(serializedObject.FindProperty("tt"));

            E.Separator();
            E.LabelField("Note Configuration", EditorStyles.boldLabel);

            s.noteTime = (ENoteTime)E.EnumPopup("Note Time", s.noteTime);
            E.PropertyField(serializedObject.FindProperty("HoldNote"));

            foldout = E.Foldout(foldout, "Note Chances");
            if (foldout)
            {
                if (s.NoteWeights.Count == 0) s.NoteWeights = Synth.GenerateNoteWeights(new int[13]);

                foreach (ENote note in System.Enum.GetValues(typeof(ENote)))
                {
                    s.NoteWeights[(int)note].Weight = E.IntSlider(Synth.Notes[(int)note], s.NoteWeights[(int)note].Weight, 0, 10);
                }
            }

            E.PropertyField(serializedObject.FindProperty("IgnoreTurnTableSilence"));

            E.Separator();
            E.LabelField("Sound Configuration", EditorStyles.boldLabel);

            E.PropertyField(serializedObject.FindProperty("Gain"));

            E.Separator();

            E.PropertyField(serializedObject.FindProperty("Instrument"));
            E.PropertyField(serializedObject.FindProperty("Envelope"));


            serializedObject.ApplyModifiedProperties();

        }
    }
}