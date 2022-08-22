using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Break Note")]
    public class ScriptableBreakNoteImprovisation : ScriptableImprovisation
    {
        public IntReference TimesToDuplicate;
        protected override Improvisation Create()
        {
            return new BreakNoteImprovisation(
                NoteSelection.Get(), 
                BarSelection.Get(), 
                TimesToDuplicate.Value
            );
        }
    }

}