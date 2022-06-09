using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Composite Bar")]
    public class CompositeBarIntervals : BarIntervals
    {
        public List<BarIntervals> Bars;

        public override bool IsValid 
        { 
            get { return Bars.All(b => b.IsValid); }
        }

        public override float GetNoteTime(int bpm, int i)
        {
            i = i % Bars.Sum(b => b.Notes.Length);
            int noteCount = 0;
            for (int j = 0; j < Bars.Count; j++)
            {
                var bar = Bars[j];
                int nNotes = bar.Notes.Length;
                if (i <= (noteCount + nNotes) - 1)
                {
                    return bar.GetNoteTime(bpm, i-noteCount);
                }
                
                noteCount += nNotes;
            }

            throw new System.Exception("Couldn't get note time. Index for sub-bar not found.");
        }

        public override float GetBarTime(int bpm)
        {
            return Bars.Sum(b=>b.GetBarTime(bpm));
        }
    }
}
