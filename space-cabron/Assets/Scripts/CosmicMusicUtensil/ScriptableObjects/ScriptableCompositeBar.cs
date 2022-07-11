using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Composite Bar")]
    public class ScriptableCompositeBar : ScriptableObject, IBar
    {
        public List<ScriptableBar> Bars = new List<ScriptableBar>();
        public bool IsValid => new CompositeBarProxy(Bars).IsValid;
        public int Length => new CompositeBarProxy(Bars).Length;

        public Note GetNote(int i)
        {
            return new CompositeBarProxy(Bars).GetNote(i);
        }

        public float GetTotalTime(int bpm)
        {
            return new CompositeBarProxy(Bars).GetTotalTime(bpm);
        }
    }
}