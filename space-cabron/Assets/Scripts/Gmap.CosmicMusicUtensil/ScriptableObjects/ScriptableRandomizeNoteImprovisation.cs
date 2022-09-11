using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Randomize Note")]
    public class ScriptableRandomizeNoteImprovisation : ScriptableImprovisation
    {
        protected override Improvisation Create()
        {
            return new RandomizeNoteImprovisation(BarSelection.Get(), NoteSelection.Get());
        }
    }
}