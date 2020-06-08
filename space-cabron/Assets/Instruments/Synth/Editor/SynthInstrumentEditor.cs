using UnityEditor;
using UnityEngine;

namespace SC
{
    using E = EditorGUILayout;

    public static class SynthInstrumentEditor
    {
        public static void DrawInspector(ref SynthInstrument inst, 
                                        ref bool expand, 
                                        ref Vector2 modulatorsScroll,
                                        Material mat)
        {
            E.Separator();

            ShitesizerEditor.DrawWave(new Vector2(100f, 100f), inst.Sample, mat);

            var s = EditorStyles.foldout;
            s.fontStyle = FontStyle.Bold;

            if (expand = E.Foldout(expand, "Oscillators", s))
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < inst.oscs.Length; i++)
                {
                    MainOscillator o = inst.oscs[i];
                    if (DrawMainOscInspector(ref o, mat))
                    {
                        ArrayUtility.RemoveAt(ref inst.oscs, i--);
                    }
                }
                //    if (GUILayout.Button("Remove Oscillator"))
                //    {
                //        ArrayUtility.RemoveAt(ref inst.oscs, i--);
                //    }

                //    E.EndScrollView();

                //    E.Space();
                //    E.Separator();
                //}

                if (GUILayout.Button("Add Oscillator"))
                {
                    ArrayUtility.Add(ref inst.oscs, new MainOscillator());
                }

                EditorGUI.indentLevel--;
            }
        }

        public static void DrawOscInspector(ref Oscillator osc)
        {
            osc.Wave = (Oscillator.EWave)E.EnumPopup("Wave", osc.Wave);
        }

        public static bool DrawMainOscInspector(ref MainOscillator osc, Material mat)
        {
            DrawOscInspector(ref osc.Oscillator);
            E.BeginHorizontal();

            ShitesizerEditor.DrawWave(new Vector2(50f, 50f), osc.Sample, mat);

            E.BeginVertical();

            /*
            osc.FrequencyFactor = E.Slider("Hz Multiplier",
                osc.FrequencyFactor,
                0f, 5f);
            */
            if (osc.FrequencyFactor == null) osc.FrequencyFactor = new ControlledFloat(1f);
            ControlledFloatInspector(ref osc.FrequencyFactor, "Hz Factor", 0f, 5f, 0f, 5f);

            /*
            osc.Amplitude = E.Slider("Amplitude",
                osc.Amplitude,
                0f, 1f);
            */
            if (osc.Amplitude == null) osc.Amplitude = new ControlledFloat(1f);
            ControlledFloatInspector(ref osc.Amplitude, "Amplitude", 0f, 1f, 0f, 1f);

            E.EndVertical();

            bool delete = GUILayout.Button("x", GUILayout.MaxWidth(20f));

            E.EndHorizontal();

            E.Space();

            return delete;
        }

        public static bool ControlledFloatInspector(ref ControlledFloat f, 
            string name, 
            float min, 
            float max,
            float driverMin,
            float driverMax)
        {
            if (f.Controlled)
            {
                E.MinMaxSlider(name, ref f.ValueRange.x, ref f.ValueRange.y, driverMin, driverMax);
                f.ValueRange = E.Vector2Field("Values", f.ValueRange);
            }
            else
            {
                f.Value = E.Slider(name, (float)f.Value, min, max);
            }
            f.Controlled = E.Toggle("Drive: ", f.Controlled);
            if (f.Controlled)
            {
                f.Driver = (EControlledFloatDriver)E.EnumPopup("Driver", f.Driver);
                f.Invert = E.Toggle("Invert", f.Invert);
            }
            E.Space();
            return false;
        }
        
    }
}