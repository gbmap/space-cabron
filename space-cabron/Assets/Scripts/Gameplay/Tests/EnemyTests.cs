using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyTests
{
    public static TestCaseData[] GetEnemies()
    {
        // return new TestCaseData[] {
        //     new TestCaseData(
        //         AssetDatabase.LoadAssetAtPath<LevelConfiguration>("Assets/Data/Levels/Level0.asset")
        //     ).Returns(null)
        // };

        var objs = Resources.LoadAll<GameObject>("Enemies/");
        return objs.Select(o => new TestCaseData(o).Returns(null)).ToArray();
    }


    // A Test behaves as an ordinary method
    // [TestCase("Assets/Actor/Enemy/Cannon/EnemyCannon.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Dasher/EnemyDasher.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Thug/EnemyThug.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Follow/EnemyFollow.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Mine/EnemyMine.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Fork/EnemyFork.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Jetpack/EnemyJetpack.prefab", ExpectedResult=null)]
    // [TestCase("Assets/Actor/Enemy/Mine/EnemyMine.prefab", ExpectedResult=null)]

    [Test, TestCaseSource(nameof(GetEnemies))]
    public IEnumerator SpawningEnemyThrowsNoException(GameObject enemy)
    {
        var instance = GameObject.Instantiate(enemy, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        GameObject.DestroyImmediate(instance);
    }
}
