using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ZapperzCurtisMarcher))]
public class ZapperzCurtisMarcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate New Wave"))
        {
            (target as ZapperzCurtisMarcher).GenerateNewBar();
        }
    }
}
