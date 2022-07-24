using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class ScriptableImprovisation : ScriptableObject
    {
        public ScriptableSelectionStrategy BarSelection;
        public ScriptableSelectionStrategy NoteSelection;
        public abstract Improvisation Get();
    }

    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Reverse Melody")]
    public class ScriptableReverseMelodyImprovisation : ScriptableImprovisation
    {
        public override Improvisation Get()
        {
            return new ReverseMelodyImprovisation(BarSelection.Get());
        }
    }
}