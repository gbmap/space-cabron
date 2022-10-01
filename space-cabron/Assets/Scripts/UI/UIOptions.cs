using System.Collections;
using System.Collections.Generic;
using SpaceCabron.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SpaceCabron.UI {
    public class UIOptions : MonoBehaviour
    {
        [SerializeField] Slider volumeSlider;
        [SerializeField] TMPro.TMP_Dropdown screenTypeDropdown;
        [SerializeField] Toggle fullscreenToggle;

        GameConfig config;

        void OnEnable() {
            config = Resources.Load<GameConfig>("GameConfig");
            volumeSlider.value = config.Volume;
            screenTypeDropdown.value = (int)config.ScreenType;
            fullscreenToggle.isOn = config.Fullscreen;

            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            screenTypeDropdown.onValueChanged.AddListener(OnScreenTypeChanged);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }

        private void OnFullscreenChanged(bool arg0)
        {
            config.Fullscreen = arg0;
            GameConfig.ApplyConfig(config);
        }

        private void OnScreenTypeChanged(int index)
        {
            config.ScreenType = (GameConfig.EScreenType)index;
            GameConfig.ApplyConfig(config);
        }

        private void OnVolumeChanged(float value)
        {
            config.Volume = value;
            GameConfig.ApplyConfig(config);
        }
    }
}