using Frictionless;
using SpaceCabron.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gmap.Gameplay
{
    public class LevelLoader
    {
        static Scene activeScene;
        public static LevelConfiguration CurrentLevelConfiguration { get; private set; }
        private static bool KeepOldScene;
        public static void Load(LevelConfiguration level)
        {
            // level = level.Clone();
            MessageRouter.Reset();
            AsyncOperation aOp = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
            aOp.completed += (AsyncOperation op) => { Callback_OnGameplaySceneLoaded(op, level); };
        }

        private static void Callback_OnGameplaySceneLoaded(AsyncOperation op, LevelConfiguration level)
        {
            UnloadOtherScenes();
            MessageRouter.RaiseMessage(new MsgLevelStartedLoading());

            if (level != null)
            {
                Debug.Log($"Configuring level {level.name}.");
                RenderSettings.skybox = level.Background.Material;
            }

            ConfigureLevelConfigurablesWithLevelConfiguration(level);
            PlayBeginLevelAnimationOnPlayers(() => MessageRouter.RaiseMessage(new MsgLevelFinishedLoading{}));
            CurrentLevelConfiguration = level;
        }

        private static void UnloadOtherScenes()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
            // SceneManager.sceneCount-1 because we don't want to unload the last 
            // loaded scene.
            for (int i = 0; i < SceneManager.sceneCount - 1; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (ShouldUnloadScene(scene))
                    SceneManager.UnloadSceneAsync(scene);
            }
        }

        private static bool ShouldUnloadScene(Scene s)
        {
            return !s.name.ToLower().Contains("test");
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
            GameObject.FindObjectOfType<RunAnimationOnPlayers>()
                      .PlayAnimation<BeginLevelBrain>(OnEnded);
        }

    }
}