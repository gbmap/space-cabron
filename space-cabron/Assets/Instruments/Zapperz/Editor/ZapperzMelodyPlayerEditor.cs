using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ZapperzMelodyPlayer))]
public class ZapperzMelodyPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate New Melody"))
        {
            (target as ZapperzMelodyPlayer).GenerateMelody();
        }
    }
}
