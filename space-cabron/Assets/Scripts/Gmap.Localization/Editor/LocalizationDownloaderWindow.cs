using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Gmap.Localization
{
    public class LocalizationDownloaderWindow : EditorWindow
    {
        [MenuItem("Localization/Localization Downloader")]
        public static void ShowWindow()
        {
            LocalizationDownloaderWindow window = GetWindow<LocalizationDownloaderWindow>();
        }

        public void CreateGUI()
        {
            TextField urlField = new TextField("URL");
            urlField.value = "https://docs.google.com/spreadsheets/d/14LFhH_oD3wJ3hVdMmllGarb9nqwPTA9uGexunvKqDCE/export?format=csv&usp=sharing";
            rootVisualElement.Add(urlField);

            Button downloadButton = new Button();
            downloadButton.text = "Download";
            downloadButton.RegisterCallback<ClickEvent>(evt => {
                SpreadsheetDownloader downloader = new SpreadsheetDownloader(urlField.value);
                var result = downloader.Download();
                if (result.Success)
                {
                    LocalizationDatabase[] databases = LocalizationDatabaseFactory.FromDataTable(result.Result);
                    foreach (LocalizationDatabase database in databases)
                    {
                        AssetDatabase.CreateAsset(database, $"Assets/Resources/Localization/{database.LanguageKey}.asset");
                        AssetDatabase.SaveAssets();
                    }
                }
            });
            rootVisualElement.Add(downloadButton);
        }
    }
}