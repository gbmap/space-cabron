using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Tremolo")]
    public class ScriptableTremoloImprovisation : ScriptableImprovisation
    {
        public IntReference Steps;
        public IntReference Beams; 

        public override Improvisation Get()
        {
            return new TremoloImprovisation(NoteSelection.Get(), BarSelection.Get(), Beams.Value, Steps.Value);
        }
    }
}