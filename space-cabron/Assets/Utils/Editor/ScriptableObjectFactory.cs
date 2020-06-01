using UnityEngine;
using System.Collections;
using UnityEditor;

// This class is just used to implement methods to create ScriptableObjects in Unity.
public static class ScriptableObjectFactory
{
    static T CreateScriptableObject<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        ProjectWindowUtil.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

    [MenuItem("Assets/Create/Scriptable Object")]
    public static ScriptableObject CreateScriptableObject()
    {
        return CreateScriptableObject<ScriptableObject>(AssetDatabase.GetAssetPath(Selection.activeObject) + "/ScriptableObject.asset");
    }
}
