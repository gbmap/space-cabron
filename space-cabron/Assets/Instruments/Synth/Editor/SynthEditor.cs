using UnityEditor;
using UnityEngine;


namespace SC {

    using E = EditorGUILayout;

    [CustomEditor(typeof(Synth))]
    public class SynthEditor : Editor
    {
        bool noteChances = false;
        bool instruments = false;
        Vector2 modulatorsScroll = new Vector2(0, 0);

        Material mat;

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnEnable()
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
        }

        private void OnDisable()
        {
            DestroyImmediate(mat);
        }

        public override void OnInspectorGUI()
        {
            var s = target as Synth;

            ShitesizerEditor.DrawWave(Vector2.one * 100f, s.PrevData, mat);

            E.Space();
            E.Separator();
            E.LabelField("Beat", EditorStyles.boldLabel);

            E.PropertyField(serializedObject.FindProperty("audioSource"));
            E.PropertyField(serializedObject.FindProperty("NoteSequencer"));

            s.noteTime = (ENoteTime)E.EnumPopup("Note Time", s.noteTime);

            E.Space();
            E.Separator();
            E.LabelField("Melody", EditorStyles.boldLabel);

            E.PropertyField(serializedObject.FindProperty("HoldNote"));
            s.Octave = E.IntSlider("Octave", s.Octave, 1, 8);

            E.Space();
            E.Separator();
            E.LabelField("Sound Configuration", EditorStyles.boldLabel);
            E.PropertyField(serializedObject.FindProperty("Gain"));

            E.Space();
            E.Separator();
            E.LabelField("Instrument", EditorStyles.boldLabel);
                     
            SynthInstrumentEditor.DrawInspector(ref s.Instrument, ref instruments, ref modulatorsScroll, mat);

            E.PropertyField(serializedObject.FindProperty("Envelope"));

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();

        }
    }
}