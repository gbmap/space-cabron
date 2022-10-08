using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class ConfigBootstrap : MonoBehaviour
    {
        void Start()
        {
            GameConfig gc = Resources.Load<GameConfig>("GameConfig");
            if (!System.IO.File.Exists(Application.persistentDataPath+"/dummy")) {
                gc.Fullscreen = false;
                gc.Resolution = new Vector2(800, 600);
                gc.ScreenType = GameConfig.EScreenType.EightBySix;
                gc.Volume = 0.5f;
                System.IO.File.WriteAllText(Application.persistentDataPath + "/dummy", "");
            }
            // GameConfig.ApplyConfig(gc);
        }
    }
}