using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Transpose Note")]
    public class ScriptableTransposeNoteImprovisation : ScriptableImprovisation
    {
        public int Steps = 1;
        public override Improvisation Get()
        {
            return new TransposeNoteImprovisation(NoteSelection.Get(), BarSelection.Get(), Steps);
        }
    }
}