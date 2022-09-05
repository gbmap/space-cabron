
using System.Collections;
using System.Linq;
using Gmap.Gameplay;
using NUnit.Framework;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Bosses;
using SpaceCabron.Gameplay.Level;
using UnityEngine;
using UnityEngine.TestTools;

public class BossTests 
{
    public static TestCaseData[] GetBossLevels()
    {
        var objs = Resources.LoadAll<BossLevelConfiguration>("Levels/");
        return objs.Select(o => new TestCaseData(o).Returns(null)).ToArray();
    }

    [TearDown]
    public void Destroy()
    {
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("Enemy"), e => GameObject.Destroy(e));
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
        BossBehaviour boss = GameObject.FindObjectOfType<BossBehaviour>();
        Assert.IsTrue(Vector3.Distance(boss.transform.position, Vector3.up * 2f) > 0.02f);
        yield return new WaitForSeconds(10f);
        Assert.IsTrue(Vector3.Distance(boss.transform.position, Vector3.up * 2f) < 0.10f);
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
