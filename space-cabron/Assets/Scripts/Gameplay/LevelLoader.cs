using System.Collections;
using Frictionless;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Level;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gmap.Gameplay
{
    public class LevelLoader
    {
        public static BaseLevelConfiguration CurrentLevelConfiguration { get; private set; }
        private static bool KeepOldScene;

        private static MonoBehaviour coroutineStarter;
        public static MonoBehaviour CoroutineStarter
        {
            get
            {
                if (coroutineStarter == null)
                {
                    GameObject go = new GameObject("CoroutineStarter");
                    coroutineStarter = go.AddComponent<CoroutineProxy>();
                }
                return coroutineStarter;
            }
        }

        public static void Load(
            BaseLevelConfiguration level, 
            System.Action OnFinishedLoading = null
        ) {
            if (CurrentLevelConfiguration != null)
                CurrentLevelConfiguration.GetLoader().Unload();
            
            level.GetLoader(OnFinishedLoading).Load();
            CurrentLevelConfiguration = level;
        }

        public static void Reload(System.Action OnFinishedReloading=null)
        {
            CoroutineStarter.StartCoroutine(ReloadAsync(OnFinishedReloading));
        }

        private static IEnumerator ReloadAsync(System.Action OnFinishedReloading)
        {
            yield return null;
            Load(CurrentLevelConfiguration, OnFinishedReloading);
        }

        private static void ConfigureLevelConfigurablesWithLevelConfiguration(LevelConfiguration level)
        {
            Scene gameplayScene = SceneManager.GetActiveScene();
            GameObject[] roots = gameplayScene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                ILevelConfigurable<LevelConfiguration>[] lc = roots[i].GetComponentsInChildren<ILevelConfigurable<LevelConfiguration>>(true);
                System.Array.ForEach(lc, (ILevelConfigurable<LevelConfiguration> l) => { l.Configure(level); });
            }
        }

        private static void PlayBeginLevelAnimationOnPlayers(System.Action OnEnded)
        {
            var anim = GameObject.FindObjectOfType<RunAnimationOnPlayers>();
            if (anim != null)
                anim.PlayAnimation<BeginLevelBrain>(OnEnded);
            else
                OnEnded();
        }

    }
}