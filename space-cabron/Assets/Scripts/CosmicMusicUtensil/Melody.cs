using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [System.Serializable]
    public class Melody : Scale
    {
        public ENote GetNote(ENote root, Scale scale, int i)
        {
            i = Bar.MathMod(i,Intervals.Length);
            int scaleInterval = Intervals[i];

            if (scaleInterval == -1)
                return ENote.None;

            return scale.GetNote(root, scaleInterval);
        }
    }
}

