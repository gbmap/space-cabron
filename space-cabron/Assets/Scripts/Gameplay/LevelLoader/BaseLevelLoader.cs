using System;
using System.Collections;
using Frictionless;
using Gmap;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCabron.Gameplay.Level
{
    public interface ILevelConfiguration : ICloneable<ILevelConfiguration>
    {
        ILevelLoader GetLoader(System.Action OnFinishedLoading =null);
    }

    public abstract class BaseLevelConfiguration : ScriptableObject, ILevelConfiguration
    {
        public abstract ILevelLoader GetLoader(System.Action OnFinishedLoading =null);
        public abstract ILevelConfiguration Clone();

        public BaseLevelConfiguration NextLevel;
    }

    public interface ILevelLoader
    {
        void Load();
        void Unload();
    }

    public class BaseLevelLoader<ConfigurationType> : ILevelLoader
    {
        protected ConfigurationType levelConfiguration;
        protected MonoBehaviour coroutineStarter;
        protected string sceneName;
        System.Action onFinishedLoading;

        public BaseLevelLoader(
            ConfigurationType levelConfiguration,
            MonoBehaviour coroutineStarter,
            string sceneName,
            System.Action OnFinishedLoading
        ) {
            this.levelConfiguration = levelConfiguration;
            this.coroutineStarter = coroutineStarter;
            this.sceneName = sceneName.ToLower();
            this.onFinishedLoading = OnFinishedLoading;
        }

        public void Load()
        {
            coroutineStarter.StartCoroutine(LoadAsync());
        }

        protected virtual IEnumerator LoadAsync()
        {
            int gameplaySceneIndex = GetSceneIndex(sceneName);
            bool gameplaySceneWasLoaded = gameplaySceneIndex != -1;
            if (!gameplaySceneWasLoaded)
            {
                SceneManager.LoadScene(
                    "Gameplay", 
                    LoadSceneMode.Additive
                );
                gameplaySceneIndex = GetSceneIndex(sceneName);
            }

            yield return WaitForSceneToBeLoaded(SceneManager.GetSceneAt(gameplaySceneIndex));

            UnloadOtherScenes();
            gameplaySceneIndex = GetSceneIndex(sceneName);
            MessageRouter.RaiseMessage(new MsgLevelStartedLoading());

            ConfigureLevel();
            ConfigureLevelConfigurablesWithConfiguration(
                levelConfiguration,
                gameplaySceneIndex
            );

            if (!gameplaySceneWasLoaded)
            {
                PlayBeginLevelAnimationOnPlayers(() => {
                    Finish();
                });
            }
            else
                Finish();

        }

        private int GetSceneIndex(string name)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.name.ToLower().Contains(name))
                    return i;
            }
            return -1;
        }

        private IEnumerator WaitForSceneToBeLoaded(Scene scene)
        {
            while (!scene.isLoaded)
                yield return null;
        }

        private void UnloadOtherScenes()
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

        private bool ShouldUnloadScene(Scene scene)
        {
            return !scene.name.ToLower().Contains("test");
        }

        private static void ConfigureLevelConfigurablesWithConfiguration(
            ConfigurationType level,
            int sceneIndex
        ) {
            Scene gameplayScene = SceneManager.GetSceneAt(sceneIndex);
            GameObject[] roots = gameplayScene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                ILevelConfigurable<ConfigurationType>[] lc = roots[i].GetComponentsInChildren<ILevelConfigurable<ConfigurationType>>(true);
                System.Array.ForEach(lc, (ILevelConfigurable<ConfigurationType> l) => { l.Configure(level); });
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

        protected virtual void ConfigureLevel() {}

        private void Finish()
        {
            MessageRouter.RaiseMessage(new MsgLevelFinishedLoading{});

            EnemySpawner spawner = GameObject.FindObjectOfType<EnemySpawner>();
            spawner.shouldSpawn = false;

            this.onFinishedLoading?.Invoke();
        }

        public void Unload()
        {
            throw new System.NotImplementedException();
        }
    }

    public class EnemyLevelLoader : BaseLevelLoader<LevelConfiguration>
    {
        public EnemyLevelLoader(
            LevelConfiguration levelConfiguration, 
            MonoBehaviour coroutineStarter,
            System.Action onFinishedLoading=null
        ) : base(levelConfiguration, coroutineStarter, "Gameplay", onFinishedLoading) {}

        protected override void ConfigureLevel()
        {
            base.ConfigureLevel();
            RenderSettings.skybox = levelConfiguration.Background.Material;
        }
    }
}