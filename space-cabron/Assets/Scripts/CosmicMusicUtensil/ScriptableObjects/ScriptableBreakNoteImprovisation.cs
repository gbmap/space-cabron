using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Break Note")]
    public class ScriptableBreakNoteImprovisation : ScriptableImprovisation
    {
        public int TimesToDuplicate = 1;
        public override Improvisation Get()
        {
            return new BreakNoteImprovisation(NoteSelection.Get(), BarSelection.Get(), TimesToDuplicate);
        }
    }

}