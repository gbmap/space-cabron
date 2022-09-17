using UnityEngine;
using AudioHelm;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Gmap.HelmSynthGenerator
{
    public class HelmModulation
    {
        public string Source;
        public string Destination;
        public FloatParameter Amount;
        public HelmModulation(string source, string destination, FloatParameter amount)
        {
            Source = source;
            Destination = destination;
            Amount = amount;
        }
    }

    public class HelmSynthGenerator
    {
        GeneratorProfile profile;

        bool onlyExistingFields;
        float randomRange = 0.1f;

        public HelmSynthGenerator(
            GeneratorProfile profile,
            bool onlyExistingFields = true,
            float randomRange = 0.1f
        )
        {
            this.profile = profile;
            this.onlyExistingFields = onlyExistingFields;
            this.randomRange = randomRange;
        }

        public HelmPatchSettings Generate(HelmPatchSettings settings = null)
        {
            if (settings == null)
                settings = new HelmPatchSettings();
            BaseHelmParameters parameters = profile.Parameters;

            var fields = settings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(float))
                {
                    bool hasValue = parameters.HasValue(field.Name);
                    float currentValue = ((float)field.GetValue(settings));
                    bool existsInOriginalSynth = currentValue != 0f;
                    bool shouldChange = hasValue && (!onlyExistingFields || existsInOriginalSynth);
                    if (shouldChange)
                    {
                        float newValue = parameters.GetValue(field.Name);
                        if (onlyExistingFields)
                        {
                            Vector2 range = parameters.GetRange(field.Name);
                            float mid = (range.x + range.y) / 2f;
                            range.x = (range.x - mid) * randomRange;
                            range.y = (range.y - mid) * randomRange;
                            newValue = currentValue + Random.Range(range.x, range.y);
                        }
                        field.SetValue(settings, newValue);
                    }
                }
            }
            settings.volume = 1f;

            int numberOfModulations = 0;
            if (parameters.HasValue("num_modulations"))
            {
                numberOfModulations = Mathf.RoundToInt(
                    parameters.GetValue("num_modulations")
                );
                // numberOfModulations = Mathf.Min(
                //     Mathf.RoundToInt(parameters.GetValue("num_modulations")),
                //     parameters.Modulations.Select(m => m.Destination).Distinct().Count()
                // );
            }

            List<string> destinations = new List<string>();
            List<HelmModulationSetting> modulationSettings = new List<HelmModulationSetting>();

            var existingModulations = settings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.FieldType == typeof(HelmModulationSetting[]))
                .FirstOrDefault().GetValue(settings) as HelmModulationSetting[];


            for (int i = 0; i < numberOfModulations; i++)
            {
                int tries = 0;
                var modulations = parameters.Modulations;

                var modulation = modulations[Random.Range(0, modulations.Count)];
                bool hasSource = HelmPatchSettings.kModulationSources.Contains(modulation.Source);
                bool hasDestination = HelmPatchSettings.kModulationDestinations.Contains(modulation.Destination);
                do
                {
                    modulation = modulations[Random.Range(0, modulations.Count)];
                    hasSource = HelmPatchSettings.kModulationSources.Contains(modulation.Source);
                    hasDestination = HelmPatchSettings.kModulationDestinations.Contains(modulation.Destination);
                } while ((!hasSource || !hasDestination || destinations.Contains(modulation.Destination)) && (++tries < 100));

                if (hasSource && hasDestination)
                {
                    destinations.Add(modulation.Destination);

                    var ms = new HelmModulationSetting();
                    ms.source = modulation.Source;
                    ms.destination = modulation.Destination;
                    ms.amount = modulation.Amount.GetValue();
                    modulationSettings.Add(ms);
                }
            }

            settings.modulations = modulationSettings.ToArray();
            return settings;
        }

        public static string HelmSettingsToString(HelmPatchSettings settings)
        {
            var fields = settings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            string str = string.Empty;
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(float))
                {
                    str += field.Name + ": " + field.GetValue(settings) + "\n";
                }
                else if (field.FieldType == typeof(HelmModulationSetting[]))
                {
                    var modulations = (HelmModulationSetting[])field.GetValue(settings);
                    foreach (var modulation in modulations)
                    {
                        str += "Modulation: " + modulation.source + " -> " + modulation.destination + " (" + modulation.amount + ")\n";
                    }
                }
            }
            return str;
        }
    }
}