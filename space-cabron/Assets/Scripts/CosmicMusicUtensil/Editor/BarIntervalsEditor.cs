using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Gmap.CosmicMusicUtensil;

/*
[CustomEditor(typeof(BarIntervals))]
public class BarIntervalsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BarIntervals b = target as BarIntervals;
        float noteSum = b.Notes.Sum(x=>1f/x);
        float timeSignature = ((float)b.TimeSignatureCount)/b.TimeSignatureType;

        if (!Mathf.Approximately(noteSum, timeSignature))
            EditorGUILayout.LabelField("Notes don't sum to Time Signature!");
    }
}
*/