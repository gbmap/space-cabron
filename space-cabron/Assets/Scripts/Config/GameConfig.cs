using System;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Gameplay {
    public class MsgOnScreenTypeChanged {
        public GameConfig.EScreenType ScreenType { get; set; }
    }

    [CreateAssetMenu(menuName="Space Cabrón/Game Config")]
    public class GameConfig : ScriptableObject
    {
        public enum EScreenType {
            EightBySix,
            NineBySixteen
        }
        public float Volume = 0.5f;
        public EScreenType ScreenType = EScreenType.EightBySix;
        public bool Fullscreen = false;
        public Vector2 Resolution = new Vector2(800, 600);

        public static void ApplyConfig(GameConfig config)
        {
            if (config == null)
                return;
            SetVolume(config);
            SetScreenType(config);
            SetFullscreen(config);
            SetResolution(config);
        }

        public static void SetResolution(GameConfig config)
        {
            Screen.SetResolution(
                (int)config.Resolution.x, 
                (int)config.Resolution.y, 
                config.Fullscreen
            );
        }

        public static void SetVolume(GameConfig config)
        {
            AudioListener.volume = config.Volume;
        }

        public static void SetScreenType(GameConfig config)
        {
            MessageRouter.RaiseMessage(new MsgOnScreenTypeChanged {
                ScreenType = config.ScreenType
            });
        }

        public static void SetFullscreen(GameConfig config)
        {
            Screen.fullScreen = config.Fullscreen;
        }

        public void OnVolumeChanged(float volume) {
            Volume = volume;
            ApplyConfig(this);
        }
    }
}