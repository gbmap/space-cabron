using UnityEngine;

namespace SpaceCabron.Gameplay {
    [CreateAssetMenu(menuName="Space Cabrón/Game Config")]
    public class GameConfig : ScriptableObject
    {
        public enum EScreenType {
            EightBySix,
            NineBySixteen
        }
        public float Volume = 0.5f;

        public static void ApplyConfig(GameConfig config) {
            if (config == null)
                return;
            AudioListener.volume = config.Volume;
        }
    }
}