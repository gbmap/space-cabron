using System.Collections;
using System.Linq;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using NUnit.Framework;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Bosses;
using SpaceCabron.Gameplay.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BossTests 
{
    public static void LoadLevelWithBoss(string bossResourcePath)
    {
        GameObjectPool pool = ScriptableObject.CreateInstance<GameObjectPool>();
        pool.SetItems(new ScriptableReferenceItem<GameObject>[] { 
            new ScriptableReferenceItem<GameObject> { 
                Weight = 1, 
                Value = Resources.Load<GameObject>(bossResourcePath) 
            }
        });

        BossLevelConfiguration bossLevelConfiguration 
            = ScriptableObject.CreateInstance<BossLevelConfiguration>();

        bossLevelConfiguration.PossibleBosses = pool;

        LevelLoader.Load(bossLevelConfiguration);
    }

    public static TestCaseData[] GetBossLevels()
    {
        var objs = Resources.LoadAll<BossLevelConfiguration>("Levels/");
        return objs.Select(o => new TestCaseData(o).Returns(null)).ToArray();
    }

    [TearDown]
    public void Destroy()
    {
        System.Array.ForEach(GameObject.FindObjectsOfType<MonoBehaviour>(), m => Object.Destroy(m.gameObject));
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("Enemy"), e => GameObject.Destroy(e));
        for (int i = 1; i < SceneManager.sceneCount; i++) {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        }
    }
    
    [UnityTest, TestCaseSource(nameof(GetBossLevels))]
    public IEnumerator LoadingLevelBossLoadsBoss(BossLevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(2f);

        Assert.IsTrue(GameObject.FindObjectOfType<BossBehaviour>() != null);
    }

    [UnityTest, TestCaseSource(nameof(GetBossLevels))]
    public IEnumerator BossPlaysIntroAnimation(BossLevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(1f);
        BossBehaviour[] bosses = GameObject.FindObjectsOfType<BossBehaviour>();
        BossBehaviour boss = bosses.FirstOrDefault(b => b.transform.parent == null);
        Assert.IsTrue(Vector3.Distance(boss.transform.position, Vector3.up * 2f) > 0.02f);
        yield return new WaitForSeconds(10f);
        Assert.IsTrue(Vector3.Distance(boss.transform.position, Vector3.up * 2f) < 0.20f);
    }

    [UnityTest, TestCaseSource(nameof(GetBossLevels))]
    public IEnumerator BossStartsFiringAfterAnimationEnded(BossLevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(1f);

        BossBehaviour[] bossBehaviours = GameObject.FindObjectsOfType<BossBehaviour>();

        Assert.IsTrue(bossBehaviours.All(b => b.IsRunning == false));

        while (GameObject.FindObjectOfType<AnimationBrain>() != null)
            yield return null;
        
        yield return new WaitForSeconds(5.0f);
        Assert.IsTrue(bossBehaviours.All(b=>b.IsRunning == true));
    }

    [UnityTest, TestCaseSource(nameof(GetBossLevels))]
    public IEnumerator BossDiesAfterAllPartsAreDestroyed(BossLevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(1f);

        BossBehaviour[] bossBehaviours = GameObject.FindObjectsOfType<BossBehaviour>();

        while (GameObject.FindObjectOfType<AnimationBrain>() != null)
            yield return null;

        yield return new WaitForSeconds(5.0f);
        Assert.IsTrue(bossBehaviours.All(b => b.IsRunning == true));

        System.Array.ForEach(bossBehaviours, b => b.GetComponent<Health>().Destroy());
        yield return new WaitForSeconds(2.0f);
        Assert.IsTrue(GameObject.FindObjectOfType<BossBehaviour>() == null);
    }

    [UnityTest, TestCaseSource(nameof(GetBossLevels))]
    public IEnumerator NextLevelIsLoadedAfterBossDies(BossLevelConfiguration level)
    {
        LevelLoader.Load(level);
        yield return new WaitForSeconds(1f);

        BossBehaviour[] bossBehaviours = GameObject.FindObjectsOfType<BossBehaviour>();

        while (GameObject.FindObjectOfType<AnimationBrain>() != null)
            yield return null;

        yield return new WaitForSeconds(5.0f);
        Assert.IsTrue(bossBehaviours.All(b => b.IsRunning == true));

        System.Array.ForEach(bossBehaviours, b => b.GetComponent<Health>().Destroy());
        yield return new WaitForSeconds(5.0f);
        Assert.IsTrue(LevelLoader.CurrentLevelConfiguration != level);
    }
}

public class BossCannonTests
{
    [UnityTest]
    public IEnumerator DestroyingNodesEnablesMainColorHealth()
    {
        BossTests.LoadLevelWithBoss("Enemies/Bosses/BossCannons");
        yield return new WaitForSeconds(10f);

        var boss = GameObject.FindObjectOfType<BossCannonBehaviour>();
        var nodes = GameObject.FindObjectsOfType<BossCannonNode>();

        foreach (var node in nodes) {
            node.GetComponent<ColorHealth>().Destroy();
        }
        yield return null;

        Assert.IsTrue(boss.GetComponentInChildren<ColorHealth>().enabled);
    }

    [UnityTest]
    public IEnumerator DestroyingNodeDestroysExtension()
    {
        BossTests.LoadLevelWithBoss("Enemies/Bosses/BossArms");
        yield return new WaitForSeconds(10f);

        Assert.AreEqual(2, GameObject.FindObjectsOfType<BossArmsArmExtension>().Length);

        var arms = GameObject.FindObjectsOfType<BossArmsFireHand>();
        foreach (var arm in arms) {
            arm.GetComponent<ColorHealth>().Destroy();
            break;
        }
        yield return null;

        Assert.AreEqual(1, GameObject.FindObjectsOfType<BossArmsArmExtension>().Length);
    }
}

public class BossArmsTests
{
    [UnityTest]
    public IEnumerator DestroyNodesEnablesMainColorHealth()
    {
        BossTests.LoadLevelWithBoss("Enemies/Bosses/BossArms");
        yield return new WaitForSeconds(10f);

        var boss = GameObject.FindObjectOfType<BossArmsBehaviour>();
        var arms = GameObject.FindObjectsOfType<BossArmsFireHand>();
        foreach (var arm in arms) {
            arm.GetComponent<ColorHealth>().Destroy();
        }
        yield return null;

        Assert.IsTrue(boss.GetComponentInChildren<ColorHealth>().enabled);
    }

    [UnityTest]
    public IEnumerator DestroyingNodeDestroysExtension()
    {
        BossTests.LoadLevelWithBoss("Enemies/Bosses/BossArms");
        yield return new WaitForSeconds(10f);

        Assert.AreEqual(2, GameObject.FindObjectsOfType<BossArmsArmExtension>().Length);

        var arms = GameObject.FindObjectsOfType<BossArmsFireHand>();
        foreach (var arm in arms) {
            arm.GetComponent<ColorHealth>().Destroy();
            break;
        }
        yield return null;

        Assert.AreEqual(1, GameObject.FindObjectsOfType<BossArmsArmExtension>().Length);
    }
}

public class BossWashingMachineTests
{
    [UnityTest]
    public IEnumerator DestroyingNodesEnablesMainColorHealth()
    {
        BossTests.LoadLevelWithBoss("Enemies/Bosses/BossWashingMachine");
        yield return new WaitForSeconds(10f);

        var boss = GameObject.FindObjectOfType<BossWashingMachineBehaviour>();
        var arms = GameObject.FindObjectsOfType<BurstShot>();
        foreach (var arm in arms) {
            arm.GetComponent<ColorHealth>().Destroy();
        }
        yield return null;

        Assert.IsTrue(boss.GetComponentInChildren<ColorHealth>().enabled);
    }

    [UnityTest]
    public IEnumerator DestroyingNodeDestroysExtension()
    {
        BossTests.LoadLevelWithBoss("Enemies/Bosses/BossWashingMachine");
        yield return new WaitForSeconds(10f);

        Assert.AreEqual(4, GameObject.FindObjectsOfType<BossArmsArmExtension>().Length);

        var arms = GameObject.FindObjectsOfType<BurstShot>();
        foreach (var arm in arms) {
            arm.GetComponent<ColorHealth>().Destroy();
            break;
        }
        yield return null;

        Assert.AreEqual(3, GameObject.FindObjectsOfType<BossArmsArmExtension>().Length);
    }
}