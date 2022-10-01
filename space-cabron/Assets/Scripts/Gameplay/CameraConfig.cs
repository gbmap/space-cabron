using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;
using UnityEngine.U2D;

namespace SpaceCabron.Gameplay {
    public class CameraConfig : MonoBehaviour
    {
        PixelPerfectCamera pixelPerfectCamera;
        void Awake() {
            pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
            GameConfig config = Resources.Load<GameConfig>("GameConfig");
            SetScreenType(config.ScreenType);

            MessageRouter.AddHandler<MsgOnScreenTypeChanged>(Callback_ScreenTypeChanged);
        }

        private void Callback_ScreenTypeChanged(MsgOnScreenTypeChanged obj)
        {
            SetScreenType(obj.ScreenType);
        }

        public void SetScreenType(GameConfig.EScreenType screenType) {
            switch (screenType) {
                case GameConfig.EScreenType.EightBySix:
                    pixelPerfectCamera.refResolutionX = 1067;
                    pixelPerfectCamera.refResolutionY = 600;
                    break;
                case GameConfig.EScreenType.NineBySixteen:
                    pixelPerfectCamera.refResolutionX = 600;
                    pixelPerfectCamera.refResolutionY = 900;
                    break;
            }
        }
    }
}