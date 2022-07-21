using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class ScriptableConverter
    {
        public static Bar FromInterface(IBar b)
        {
            Bar bar = new Bar();
            for (int i = 0; i<b.Length; i++)
                bar.AddNote(b.GetNote(i));
            return bar;
        }

        public static ScriptableBar ToScriptable(IBar bar)
        {
            var scriptableBar = ScriptableObject.CreateInstance<ScriptableBar>();
            scriptableBar.Bar = FromInterface(bar);
            return scriptableBar;
        }

        public static ScriptableCompositeBar ToScriptable(CompositeBar compositeBar)
        {
            var scriptableCompositeBar = ScriptableObject.CreateInstance<ScriptableCompositeBar>();
            scriptableCompositeBar.Bars = compositeBar.Bars.Select(b => ToScriptable(b)).ToList();
            return scriptableCompositeBar;
        }
    }
}