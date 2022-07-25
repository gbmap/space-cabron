using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gmap.Gameplay;

namespace SpaceCabron.Gameplay
{
    public class LevelLoader
    {
        static Scene activeScene;
        static LevelConfiguration CurrentLevelConfiguration;
        public static void Load(LevelConfiguration level)
        {
            activeScene = SceneManager.GetActiveScene();
            AsyncOperation aOp = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);
            aOp.completed += (AsyncOperation op) => { Callback_OnGameplaySceneLoaded(op, level); };
        }

        private static void Callback_OnGameplaySceneLoaded(AsyncOperation op, LevelConfiguration level)
        {
            SceneManager.UnloadSceneAsync(activeScene.buildIndex);
            RenderSettings.skybox = level.Background.Material;

            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                ILevelConfigurable<LevelConfiguration>[] lc = roots[i].GetComponentsInChildren<ILevelConfigurable<LevelConfiguration>>(true);
                System.Array.ForEach(lc, (ILevelConfigurable<LevelConfiguration> l) => { l.Configure(level); });
            }
        }
    }
}