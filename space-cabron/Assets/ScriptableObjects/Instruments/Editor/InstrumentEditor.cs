using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Instrument))]
public class InstrumentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnumPrefabEditor<EInstrumentAudio, AudioClip>.Draw(ref (target as Instrument).Instruments);
    }
}
