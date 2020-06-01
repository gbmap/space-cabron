using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SC
{
    using E = EditorGUILayout;

    public static class SynthInstrumentEditor
    {
        public static void DrawInspector(ref SynthInstrument inst, 
                                        ref bool expand, 
                                        ref Vector2 modulatorsScroll)
        {
            E.Separator();

            var s = EditorStyles.foldout;
            s.fontStyle = FontStyle.Bold;

            if (expand = E.Foldout(expand, "Oscillators", s))
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < inst.oscs.Length; i++)
                {
                    Oscillator o = inst.oscs[i];
                    o.Wave = (Oscillator.EWave)E.EnumPopup("Wave", o.Wave);
                    o.Amplitude = E.Slider("Amplitude", o.Amplitude, 0f, 1f);
                    o.FrequencyFactor = E.Slider("Frequency Multiplier", o.FrequencyFactor, 0f, 10f);

                    EditorGUI.indentLevel++;
                    E.LabelField("Modulators");

                    modulatorsScroll = E.BeginScrollView(modulatorsScroll);

                    if (o.Modulators == null) o.Modulators = new LFModulator[0];
                    
                    for (int j = 0; j < o.Modulators.Length; j++)
                    {
                        if (DrawModulatorInspector(ref o.Modulators[j]))
                        {
                            ArrayUtility.RemoveAt(ref o.Modulators, j--);
                        }
                    }
                    if (GUILayout.Button("Add Modulator"))
                    {
                        ArrayUtility.Add(ref o.Modulators, new LFModulator());
                    }
                    EditorGUI.indentLevel--;

                    E.Space();
                    E.Separator();

                    if (GUILayout.Button("Remove Oscillator"))
                    {
                        ArrayUtility.RemoveAt(ref inst.oscs, i--);
                    }

                    E.EndScrollView();

                    E.Space();
                    E.Separator();


                }

                if (GUILayout.Button("Add Oscillator"))
                {
                    ArrayUtility.Add(ref inst.oscs, new Oscillator());
                }

                EditorGUI.indentLevel--;
            }
        }

        public static bool DrawModulatorInspector(ref LFModulator mod)
        {
            
            mod.Amplitude = E.Slider("Amplitude" ,mod.Amplitude, 0f, 1f);
            mod.Frequency = E.Slider("Frequency", mod.Frequency, 0f, 20f);
            bool r = GUILayout.Button("Remove");
            E.Space();
            return r;
        }
    }
}