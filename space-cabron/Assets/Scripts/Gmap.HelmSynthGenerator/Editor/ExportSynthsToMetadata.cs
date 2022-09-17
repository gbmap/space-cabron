using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using AudioHelm;
using Gmap.ScriptableReferences;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gmap.HelmSynthGenerator
{
    public class ExportSynthsToMetadata : EditorWindow
    {
        public TextAssetPool SynthPool;

        public const string FOLDER_METADATA = "Datasets";
        public const string FOLDER_GENERATED_INSTRUMENTS_METADATA = "GeneratedInstrumentsMetadata";

        [MenuItem("Gmap/Export Synths to Metadata")]
        public static void ShowWindow()
        {
            ExportSynthsToMetadata window = GetWindow<ExportSynthsToMetadata>();
        }

        public void CreateGUI() {
            Label label = new Label("Export");
            rootVisualElement.Add(label);

            TextField textMetadataFilename = new TextField("Metadata Filename");
            textMetadataFilename.value = $"{FOLDER_METADATA}/synths_metadata";
            rootVisualElement.Add(textMetadataFilename);

            ObjectField objectField = new ObjectField("Synth Pool");
            objectField.objectType = typeof(TextAssetPool);
            objectField.RegisterValueChangedCallback<Object>(evt => {
                SynthPool = evt.newValue as TextAssetPool;
            });
            rootVisualElement.Add(objectField);

            Button exportButton = new Button();
            exportButton.text = "Export";
            exportButton.RegisterCallback<ClickEvent>(evt => {
                if (SynthPool == null) {
                    return;
                }

                DataTable dataTable = GenerateDataTable();
                ExportToCsv(dataTable, textMetadataFilename.value);
            });
            rootVisualElement.Add(exportButton);

            Label importLabel = new Label("Import");
            rootVisualElement.Add(importLabel);

            ObjectField importObjectField = new ObjectField("New Synths Csv");
            importObjectField.objectType = typeof(TextAsset);
            rootVisualElement.Add(importObjectField);

            Button importButton = new Button();
            importButton.text = "Import";
            importButton.RegisterCallback<ClickEvent>(evt => {
                TextAsset newSynthsCsv = importObjectField.value as TextAsset;
                if (newSynthsCsv == null) {
                    return;
                }

                AudioHelm.HelmPatchSettings[] settings = CsvToPatchSettings(newSynthsCsv.text);
                SavePatchSettings(settings);
            });
            rootVisualElement.Add(importButton);

            Label labelPoolGenerator = new Label("Synth TextAssetPool generator");
            rootVisualElement.Add(labelPoolGenerator);

            TextField textSynthsPath = new TextField("Synths Path");
            textSynthsPath.value = "Assets/AudioHelm/Presets/";
            rootVisualElement.Add(textSynthsPath);

            TextField outputFile = new TextField("Output File");
            outputFile.value = $"{FOLDER_METADATA}/Patches";
            rootVisualElement.Add(outputFile);

            TextField textExtension = new TextField("Extension");
            textExtension.value = "json";
            rootVisualElement.Add(textExtension);

            Button generateAllSynthsPool = new Button();
            generateAllSynthsPool.text = "Generate All Synths Pool";
            generateAllSynthsPool.RegisterCallback<ClickEvent>(evt => {
                GeneratePoolFromSynthTextAssets(
                    textSynthsPath.value,
                    outputFile.value,
                    textExtension.value
                );
            });
            rootVisualElement.Add(generateAllSynthsPool);
        }

        private void GeneratePoolFromSynthTextAssets(
            string path,
            string outputFilename,
            string extension
        ) {
            string[] files = Directory.GetFiles(path, $"*.{extension}", SearchOption.AllDirectories);
            List<TextAsset> textAssets = new List<TextAsset>();
            TextAsset[] objectAssets = files.Select(f => AssetDatabase.LoadAssetAtPath<TextAsset>(f)).ToArray();
            TextAsset[] assets = objectAssets;
            if (assets.Length == 0) {
                Debug.Log("No assets found");
                return;
            }
            textAssets.AddRange(assets);

            TextAssetPool generatedSynths = ScriptableObject.CreateInstance<TextAssetPool>();
            generatedSynths.SetItems(textAssets.Select(t => new ScriptableReferenceItem<TextAsset> { Value = t, Weight = 1 }).ToArray());

            AssetDatabase.CreateAsset(generatedSynths, $"{path}/{outputFilename}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private AudioHelm.HelmPatchSettings[] CsvToPatchSettings(string text)
        {
            var lines = text.Split('\n');
            var columns = lines[0].Split(',');
            AudioHelm.HelmPatchSettings[] synths = new AudioHelm.HelmPatchSettings[lines.Length - 1];
            for (int i = 1; i < lines.Length-1; i++) {
                var synth = new AudioHelm.HelmPatchSettings();
                var modulations = new List<HelmModulationSetting>();
                var values = lines[i].Split(',');
                for (int j = 0; j < columns.Length; j++) {
                    var column = columns[j];
                    var value = values[j].Replace(".", ",");
                    bool isModulation = column.StartsWith("Modulation");
                    bool isModulationSource = isModulation && column.EndsWith("Source");
                    if (isModulationSource) {
                        int modulationIndex = int.Parse(column.Substring(10, 1));
                        int source= Mathf.RoundToInt(float.Parse(value));
                        if (source < 0 || source > HelmPatchSettings.kModulationSources.Length-1) {
                            continue;
                        }
                        int destination = Mathf.RoundToInt(float.Parse(values[j + 1].Replace(".", ",")));
                        if (destination < 0 || destination > HelmPatchSettings.kModulationDestinations.Length-1) {
                            continue;
                        }

                        float amount = float.Parse(values[j + 2].Replace(".", ","));

                        modulations.Add(new HelmModulationSetting{
                            source = HelmPatchSettings.kModulationSources[source],
                            destination = HelmPatchSettings.kModulationDestinations[destination],
                            amount = amount
                        });
                    }
                    else if (!isModulation) {
                        try {
                            var type = synth.GetType();
                            if (type.GetField(column) != null && !string.IsNullOrEmpty(value)) {
                                type.GetField(column).SetValue(synth, float.Parse(value));
                            }
                        } catch (System.Exception ex) {
                            Debug.LogError(ex.Message);
                        }
                    }
                }
                synth.modulations = modulations.ToArray();
                synths[i] = synth;
            }
            return synths;
        }

        private void SavePatchSettings(
            HelmPatchSettings[] settings,
            string path = "Assets/Resources/GeneratedSynths"
        ) {
            List<TextAsset> textAssets = new List<TextAsset>();
            int currentSynthCount = Resources.LoadAll<TextAsset>("GeneratedSynths/").Length;
            int i = currentSynthCount;
            foreach (HelmPatchSettings synth in settings) {
                HelmPatchFormat format = new HelmPatchFormat();
                format.synth_version = "0.0.1";
                format.license = "Whatever.";
                format.author = "Gmap";
                format.settings = synth;

                TextAsset textAsset = new TextAsset(JsonUtility.ToJson(format));
                AssetDatabase.CreateAsset(textAsset, $"{path}/{i}.asset");
                i++;

                textAssets.Add(textAsset);
            }

            TextAssetPool generatedSynths = ScriptableObject.CreateInstance<TextAssetPool>();
            generatedSynths.SetItems(textAssets.Select(t => new ScriptableReferenceItem<TextAsset> { Value = t, Weight = 1 }).ToArray());

            AssetDatabase.CreateAsset(generatedSynths, $"{path}/GeneratedSynthPool.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public DataTable GenerateDataTable() {
            var synths = SynthPool.GetItems();

            DataTable dataTable = new DataTable();
            typeof(AudioHelm.HelmPatchSettings).GetFields()
                                               .Where(f => f.FieldType == typeof(float))
                                               .ToList()
                                               .ForEach(f => {
                dataTable.Columns.Add(f.Name, typeof(float));
            });
            for (int i = 1; i <= AudioHelm.HelmPatchSettings.kMaxModulations; i++) {
                dataTable.Columns.Add($"Modulation{i}Source", typeof(int));
                dataTable.Columns.Add($"Modulation{i}Destination", typeof(int));
                dataTable.Columns.Add($"Modulation{i}Amount", typeof(float));
            }

            foreach (var synth in synths) {
                var synthData = synth.Value.text;
                var settings = JsonUtility.FromJson<AudioHelm.HelmPatchFormat>(synthData).settings;

                DataRow row = dataTable.NewRow();
                var fields = settings.GetType().GetFields().Where(f => f.FieldType == typeof(float));
                foreach (var field in fields) {
                    row[field.Name] = field.GetValue(settings);
                }

                int nModulations = Mathf.Min(
                    settings.modulations.Length,
                    AudioHelm.HelmPatchSettings.kMaxModulations
                );
                for (int i = 0; i < nModulations; i++) {
                    var modulation = settings.modulations[i];
                    int sourceIndex = AudioHelm.HelmPatchSettings.GetSourceIndex(modulation.source);
                    int destinationIndex = AudioHelm.HelmPatchSettings.GetDestinationIndex(modulation.destination);

                    row[$"Modulation{i + 1}Source"] = sourceIndex;
                    row[$"Modulation{i + 1}Destination"] = destinationIndex;
                    row[$"Modulation{i + 1}Amount"] = modulation.amount;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public void ExportToCsv(DataTable dataTable, string filename) {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<object> fields = row.ItemArray;
                sb.AppendLine(string.Join(",", fields.Select(f=>f.ToString().Replace(',', '.'))));
            }

            File.WriteAllText($"Assets/Scripts/Gmap.HelmSynthGenerator/{filename}.csv", sb.ToString());
            AssetDatabase.Refresh();
        }
    }
}