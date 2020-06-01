using System;
using UnityEditor;
using UnityEngine;

public static class EnumPrefabEditor<T, P> where T : Enum where P : UnityEngine.Object
{
    public static P[] Draw(UnityEngine.Object target, ref P[] ps)
    {
        var values = Enum.GetValues(typeof(T));

        if (ps == null) return null;

        for (int i = 0; i < values.Length; i++)
        {
            if (ps.Length-1 < i)
            {
                ArrayUtility.Add(ref ps, null);
            }

            T t = (T)values.GetValue(i);
            ps[i] = (P)EditorGUILayout.ObjectField(t.ToString(), ps[i], typeof(P), false);
        }

        EditorUtility.SetDirty(target);
        return ps;
    }
}

[CustomEditor(typeof(EnemyPrefabs))]
public class EnemyPrefabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var p = target as EnemyPrefabs;

        var values = Enum.GetValues(typeof(EEnemyType));

        if (p.prefabs == null) p.prefabs = new GameObject[values.Length];
        p.prefabs = EnumPrefabEditor<EEnemyType, UnityEngine.GameObject>.Draw(target ,ref p.prefabs);

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
