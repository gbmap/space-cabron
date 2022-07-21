using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody")]
    public class ScriptableMelody : ScriptableObject, IScale
    {
        [SerializeField]
        private Melody Melody;

        public ENote GetNote(ENote root, int i)
        {
            return Melody.GetNote(root, i);
        }

        public int GetNumberOfNotes()
        {
            return Melody.GetNumberOfNotes();
        }
    }

}