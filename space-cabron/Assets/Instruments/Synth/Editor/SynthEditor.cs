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

        public override void OnInspectorGUI()
        {
            var s = target as Synth;

            E.Space();
            E.Separator();
            E.LabelField("Beat", EditorStyles.boldLabel);

            E.PropertyField(serializedObject.FindProperty("audioSource"));

            BeatMakerEditorUtility.DrawInspector(ref s.BPM, ref s.MaxSubBeats, ref s.MaxInstrumentsPerBeat, ref s.NBeats);

            s.noteTime = (ENoteTime)E.EnumPopup("Note Time", s.noteTime);

            E.Space();
            E.Separator();
            E.LabelField("Melody", EditorStyles.boldLabel);

            E.PropertyField(serializedObject.FindProperty("HoldNote"));
            s.Octave = E.IntSlider("Octave", s.Octave, 1, 8);

            noteChances = E.Foldout(noteChances, "Note Chances");
            if (noteChances)
            {
                if (s.NoteWeights.Count == 0) s.NoteWeights = Synth.GenerateNoteWeights(new int[13]);

                foreach (ENote note in System.Enum.GetValues(typeof(ENote)))
                {
                    s.NoteWeights[(int)note].Weight = E.IntSlider(Synth.Notes[(int)note], s.NoteWeights[(int)note].Weight, 0, 10);
                }
            }

            if (GUILayout.Button("Generate New Notes"))
            {
                s.ChangePattern(true);
            }

            E.Space();
            E.Separator();
            E.LabelField("Sound Configuration", EditorStyles.boldLabel);
            E.PropertyField(serializedObject.FindProperty("Gain"));

            E.Space();
            E.Separator();
            E.LabelField("Instrument", EditorStyles.boldLabel);

            SynthInstrumentEditor.DrawInspector(ref s.Instrument, ref instruments, ref modulatorsScroll);

            E.PropertyField(serializedObject.FindProperty("Envelope"));

            serializedObject.ApplyModifiedProperties();

        }
    }
}