using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FXPrefabs))]
public class FXPrefabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var fx = target as FXPrefabs;
        fx.Explosions = EnumPrefabEditor<FX.EExplosionSize, GameObject>.Draw(target, ref fx.Explosions);
        fx.EnemyDebris = (GameObject)EditorGUILayout.ObjectField("Enemy Debris", fx.EnemyDebris, typeof(GameObject), false);
    }
}
