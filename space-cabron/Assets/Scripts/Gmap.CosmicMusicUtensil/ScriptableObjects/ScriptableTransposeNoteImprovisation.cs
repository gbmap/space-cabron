using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Transpose Note")]
    public class ScriptableTransposeNoteImprovisation : ScriptableImprovisation
    {
        public IntReference Steps;
        public bool RandomizeSign = false;
        protected override Improvisation Create()
        {
            int sign = RandomizeSign ? 1 - (System.Convert.ToInt32(Random.value > 0.5f)*2) : 1;
            return new TransposeNoteImprovisation(NoteSelection.Get(), BarSelection.Get(), Steps.Value*sign);
        }
    }
}