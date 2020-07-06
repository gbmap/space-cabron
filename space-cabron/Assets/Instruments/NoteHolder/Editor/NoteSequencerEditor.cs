using SC;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoteSequencer))]
public class NoteSequencerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var nc = target as NoteSequencer;

        DrawDefaultInspector();

        if (NoteChancesEditor<ENote>.DrawChancesInspector(ref nc.NoteWeights))
        {
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Generate New Notes"))
        {
            nc.UpdateNoteBag(nc.Marcher.NotesInBar);
        }
    }
}
