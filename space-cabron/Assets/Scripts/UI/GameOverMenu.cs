using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCabron.Gameplay
{
    public class GameOverMenu : MonoBehaviour
    {
        public void Retry()
        {
            if (LevelLoader.CurrentLevelConfiguration)
                LevelLoader.Load(Resources.Load<LevelConfiguration>("Levels/Level0"));
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Exit()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}