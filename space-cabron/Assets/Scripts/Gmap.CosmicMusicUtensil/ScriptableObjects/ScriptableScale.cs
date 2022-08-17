using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Scale")]
    public class ScriptableScale : ScriptableObject, IScale
    {
        [SerializeField]
        private Scale Scale;

        public ENote GetNote(ENote root, int i)
        {
            return Scale.GetNote(root, i);
        }

        public int GetNumberOfNotes()
        {
            return Scale.GetNumberOfNotes();
        }
    }

}