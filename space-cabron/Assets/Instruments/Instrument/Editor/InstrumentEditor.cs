using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SC
{
    using E = EditorGUILayout;

    public static class InstrumentEditor<T> where T : System.Enum
    {
        public static bool DrawChancesInspector(ref List<NoteChance> list)
        {
            bool changed = false;

            var values = System.Enum.GetValues(typeof(T));
            if (list == null || list.Count == 0)
            {
                list = new List<NoteChance>(values.Length);
                foreach (T note in values)
                {
                    list.Add(new NoteChance()
                    {
                        Note = Convert.ToInt32(note),
                        Weight = 0
                    });
                }
            }

            foreach (T note in values)
            {
                NoteChance nc = list[Convert.ToInt32(note)];

                int lastWeight = nc.Weight;
                nc.Note = Convert.ToInt32(note);
                nc.Weight = E.IntSlider(note.ToString(), nc.Weight, 0, 10);
                list[(int)(object)note] = nc;

                changed |= lastWeight != nc.Weight;
            }

            return changed;
        }
    }
}