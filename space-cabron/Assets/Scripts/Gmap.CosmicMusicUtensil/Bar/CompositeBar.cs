using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class CompositeBarProxy : IBar
    {
        public IEnumerable<IBar> Bars { get; private set; }
        public CompositeBarProxy(IEnumerable<IBar> bars)
        {
            Bars = bars;
        }

        public int Length => Bars.Sum(b=>b.Length);
        public bool IsValid => Bars.All(b=>b.IsValid);

        public Note GetNote(int i)
        {
            i = Bar.MathMod(i, Length);
            for (int j = 0; j < Bars.Count(); j++)
            {
                IBar bar = Bars.ElementAt(j);
                
                int startIndex = Bars.Take(j).Sum(b=>b.Length);
                int endIndex = startIndex + bar.Length - 1;

                if (i < startIndex || i > endIndex)
                    continue;

                return bar.GetNote(i-startIndex);
            }

            throw new System.Exception("Couldn't get note.");
        }

        public float GetTotalTime(int bpm)
        {
            return Bars.Sum(b=>b.GetTotalTime(bpm));
        }
    }

    public class CompositeBar : IBar
    {
        public List<IBar> Bars = new List<IBar>();

        public int Length => new CompositeBarProxy(Bars).Length;
        public bool IsValid => new CompositeBarProxy(Bars).IsValid;

        public Note GetNote(int i)
        {
            return new CompositeBarProxy(Bars).GetNote(i);
        }

        public float GetTotalTime(int bpm)
        {
            return new CompositeBarProxy(Bars).GetTotalTime(bpm);
        }

        public void AddBar(IBar bar)
        {
            Bars.Add(bar);
        }

        public static CompositeBar Create()
        {
            // return ScriptableObject.CreateInstance<CompositeBar>();
            return new CompositeBar();
        }
    }
}