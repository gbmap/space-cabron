using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class ScriptableImprovisation : ScriptableObject
    {
        public ScriptableSelectionStrategy BarSelection;
        public ScriptableSelectionStrategy NoteSelection;

        private Improvisation improvisationCache;
        public Improvisation Get()
        {
            if (improvisationCache == null)
                improvisationCache = Create();
            return improvisationCache;
        }

        protected abstract Improvisation Create();

        public Sprite Icon;
    }

    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Improvisations/Reverse Melody")]
    public class ScriptableReverseMelodyImprovisation : ScriptableImprovisation
    {
        protected override Improvisation Create()
        {
            return new ReverseMelodyImprovisation(BarSelection.Get());
        }
    }
}