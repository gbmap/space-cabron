using UnityEngine;


namespace SpaceCabron.Gameplay
{
    public class LoadScene : MonoBehaviour
    {
        public void Load(string sceneName) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}