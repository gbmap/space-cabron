using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Mordent")]
    public class ScriptableMordentImprovisation : ScriptableImprovisation
    {
        public IntReference Steps;
        protected override Improvisation Create()
        {
            return new MordentImprovisation(NoteSelection.Get(), BarSelection.Get(), Steps.Value);
        }
    }

}