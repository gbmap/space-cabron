using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCabron.Gameplay
{
    public class GameOverMenu : MonoBehaviour
    {
        public void Retry()
        {
            MessageRouter.RaiseMessage(new MsgOnRetry{});
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