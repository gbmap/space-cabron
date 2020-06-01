using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SC
{
    using E = EditorGUILayout;
    /*
    [CustomEditor(typeof(BeatMaker))]
    public class BeatMakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var bm = target as BeatMaker;
            //BeatMakerEditorUtility.DrawInspector(ref bm.BPM, ref bm.MaxSubBeats, ref bm.MaxInstrumentsInBeat, ref bm.BeatsInBar);
        }
    }
    */
    public static class BeatMakerEditorUtility
    {
        public static void DrawInspector(ref int BPM,
            ref int MaxSubBeats,
            ref int MaxInstrumentsPerBeat,
            ref int NBeats)
        {
            E.Space();
            E.LabelField("Beatmaker Configuration", EditorStyles.boldLabel);

            BPM = E.IntSlider("BPM", BPM, 30, 360);
            MaxSubBeats = E.IntSlider("Max Sub Beats", MaxSubBeats, 1, 8);
            MaxInstrumentsPerBeat = E.IntSlider("Max Instruments Per Beat", MaxInstrumentsPerBeat, 1, 6);
            NBeats = E.IntSlider("Number of Beats Per Bar", NBeats, 1, 16);

            E.Space();
        }
    }
}