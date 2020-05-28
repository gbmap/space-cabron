using System;
using UnityEditor;

public static class EnumPrefabEditor<T, P> where T : Enum where P : UnityEngine.Object
{
    public static void Draw(ref P[] ps)
    {
        var values = Enum.GetValues(typeof(T));

        if (ps == null) return;

        for (int i = 0; i < values.Length; i++)
        {
            if (ps.Length-1 < i)
            {
                ArrayUtility.Add(ref ps, null);
            }

            T t = (T)values.GetValue(i);
            ps[i] = (P)EditorGUILayout.ObjectField(t.ToString(), ps[i], typeof(P), false);
        }
    }
}

[CustomEditor(typeof(EnemyPrefabs))]
public class EnemyPrefabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var p = target as EnemyPrefabs;
        EnumPrefabEditor<EEnemyType, UnityEngine.GameObject>.Draw(ref p.prefabs);
    }
}
