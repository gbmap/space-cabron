using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyTests
{
    // A Test behaves as an ordinary method
    [TestCase("Assets/Actor/Enemy/Cannon/EnemyCannon.prefab", ExpectedResult=null)]
    [TestCase("Assets/Actor/Enemy/Dasher/EnemyDasher.prefab", ExpectedResult=null)]
    [TestCase("Assets/Actor/Enemy/Thug/EnemyThug.prefab", ExpectedResult=null)]
    [TestCase("Assets/Actor/Enemy/Follow/EnemyFollow.prefab", ExpectedResult=null)]
    [TestCase("Assets/Actor/Enemy/Mine/EnemyMine.prefab", ExpectedResult=null)]
    public IEnumerator SpawningEnemyThrowsNoException(string path)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var instance = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        GameObject.DestroyImmediate(instance);
    }
}
