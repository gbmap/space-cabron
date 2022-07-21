using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Composite Bar")]
    public class ScriptableCompositeBar : ScriptableObject, IBar
    {
        public List<ScriptableBar> Bars = new List<ScriptableBar>();
        
        public bool IsValid => Proxy.IsValid;
        public int Length => Proxy.Length;

        private CompositeBarProxy proxy;
        private CompositeBarProxy Proxy
        {
            get { return proxy ?? (proxy = new CompositeBarProxy(Bars)); }
        }

        public Note GetNote(int i)
        {
            return Proxy.GetNote(i);
        }

        public float GetTotalTime(int bpm)
        {
            return Proxy.GetTotalTime(bpm);
        }
    }
}