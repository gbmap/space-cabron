using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody")]
    public class Melody : Scale
    {
        public ENote GetNote(ENote root, Scale scale, int i)
        {
            i = i % Intervals.Length;
            int scaleInterval = Intervals[i];

            if (scaleInterval == -1)
                return ENote.None;

            return scale.GetNote(root, scaleInterval);
        }
    }
}

