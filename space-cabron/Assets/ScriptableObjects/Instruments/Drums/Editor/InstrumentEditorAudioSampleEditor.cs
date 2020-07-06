using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InstrumentAudioSample))]
public class InstrumentAudioSampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var insts = target as InstrumentAudioSample;
        insts.Instruments = EnumPrefabEditor<ENote, AudioClip>.Draw(target, ref insts.Instruments);
    }
}
