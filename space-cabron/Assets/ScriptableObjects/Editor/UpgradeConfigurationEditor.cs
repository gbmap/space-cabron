using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradeConfiguration))]
public class UpgradeConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var uc = target as UpgradeConfiguration;

        var values = Enum.GetValues(typeof(EUpgrade));
        if (uc.sprites == null) uc.sprites = new Sprite[values.Length];
        uc.sprites = EnumPrefabEditor<EUpgrade, Sprite>.Draw(target, ref uc.sprites);

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
