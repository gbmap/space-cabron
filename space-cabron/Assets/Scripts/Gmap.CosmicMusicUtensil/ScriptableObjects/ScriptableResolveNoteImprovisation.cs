using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Resolve Note")]
    public class ScriptableResolveNoteImprovisation : ScriptableImprovisation
    {
        public override Improvisation Get()
        {
            return new ResolveNoteImprovisation(BarSelection.Get());
        }
    }
}