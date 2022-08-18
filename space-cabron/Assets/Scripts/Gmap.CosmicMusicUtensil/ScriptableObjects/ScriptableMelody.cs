using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody")]
    public class ScriptableMelody : ScriptableObject, IScale
    {
        [SerializeField]
        private Melody Melody;

        public int GetIndex(ENote root, ENote tone)
        {
            throw new System.NotImplementedException();
        }

        public ENote GetNote(ENote root, int i)
        {
            return ENote.None;
            // return Melody.GetNote(root, i);
        }

        public int GetNumberOfNotes()
        {
            return 1;
            // return Melody.GetNumberOfNotes();
        }
    }

}