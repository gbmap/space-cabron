

using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName = "Gmap/Cosmic Music Utensil/Bar")]
    public class ScriptableBar : ScriptableObject, IBar
    {
        public Bar Bar;
        public bool IsValid => Bar.IsValid;

        public int Length => Bar.Length;

        public Note GetNote(int i)
        {
            return Bar.GetNote(i);
        }

        public float GetTotalTime(int bpm)
        {
            return Bar.GetTotalTime(bpm);
        }
    }

}