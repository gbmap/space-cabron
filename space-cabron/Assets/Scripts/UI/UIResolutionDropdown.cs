using System;
using System.Collections;
using System.Collections.Generic;
using SpaceCabron.Gameplay;
using UnityEngine;

namespace SpaceCabron.UI {
    public class UIResolutionDropdown : MonoBehaviour
    {
        TMPro.TMP_Dropdown dropdown;

        // Start is called before the first frame update
        void Start()
        {
            dropdown = GetComponent<TMPro.TMP_Dropdown>();
            dropdown.options.Clear();
            int selectedIndex = 0;
            int i = 0;
            List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
            foreach (Resolution resolution in Screen.resolutions) {
                var option = new TMPro.TMP_Dropdown.OptionData(resolution.ToString());
                if (Screen.currentResolution.width == resolution.width
                && Screen.currentResolution.height == resolution.height) {
                    selectedIndex = i;
                }
                options.Add(option);
                i++;
            }
            dropdown.AddOptions(options);
            dropdown.value = selectedIndex;
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int index)
        {
            GameConfig config = Resources.Load<GameConfig>("GameConfig");
            config.Resolution = new Vector2(
                Screen.resolutions[index].width,
                Screen.resolutions[index].height
            );
            GameConfig.ApplyConfig(config);
        }
    }
}