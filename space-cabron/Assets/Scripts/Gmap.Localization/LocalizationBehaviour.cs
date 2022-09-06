using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gmap.Localization
{
    public class LocalizationBehaviour : MonoBehaviour
    {
        public string Key;
        private string value;
        public UnityEvent<string> OnLocalizationUpdate;

        void Start()
        {
            value = GetString(Key, "en");
            OnLocalizationUpdate?.Invoke(value);
        }

        private static LocalizationDatabase Get(string languageKey)
        {
            return Resources.Load<LocalizationDatabase>($"Localization/{languageKey}");
        }

        public static string GetString(string key, string languageKey)
        {
            try
            {
                return Get(languageKey).GetString(key);
            }
            catch (System.Exception ex) {
                Debug.LogError("Localization error: " + ex.Message);
                return "LOCALIZATION ERROR";
            }
        }
    }
}