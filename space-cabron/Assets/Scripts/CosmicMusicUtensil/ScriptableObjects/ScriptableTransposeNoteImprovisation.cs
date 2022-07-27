using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Transpose Note")]
    public class ScriptableTransposeNoteImprovisation : ScriptableImprovisation
    {
        public int Steps = 1;
        public bool RandomizeSign = false;
        public override Improvisation Get()
        {
            int sign = RandomizeSign ? 1 - (System.Convert.ToInt32(Random.value > 0.5f)*2) : 1;
            return new TransposeNoteImprovisation(NoteSelection.Get(), BarSelection.Get(), Steps*sign);
        }
    }
}