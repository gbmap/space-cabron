using Gmap.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCabron.Gameplay
{
    public class GameOverMenu : MonoBehaviour
    {
        public void Retry()
        {
            if (LevelLoader.CurrentLevelConfiguration != null)
            {
                LevelLoader.Reload();
            }
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Exit()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}