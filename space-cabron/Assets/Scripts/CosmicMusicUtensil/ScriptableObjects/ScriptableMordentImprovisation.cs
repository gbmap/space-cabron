using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Mordent")]
    public class ScriptableMordentImprovisation : ScriptableImprovisation
    {
        public IntReference Steps;
        public override Improvisation Get()
        {
            return new MordentImprovisation(NoteSelection.Get(), BarSelection.Get(), Steps.Value);
        }
    }

}