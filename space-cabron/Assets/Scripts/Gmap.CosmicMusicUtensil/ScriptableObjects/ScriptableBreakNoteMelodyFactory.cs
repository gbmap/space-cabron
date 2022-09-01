using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody Factory/Break Note")]
    public class ScriptableBreakNoteMelodyFactory : ScriptableMelodyFactory
    {
        public ScriptableNotePool Root;
        public ScriptableScalePool Scale;
        public RandomIntReference Octave;
        public Vector2Int TimeSignature;
        public int NumberOfBreaks = 5;

        public override ScriptableMelodyFactory Clone()
        {
            var clone = ScriptableObject.CreateInstance<ScriptableBreakNoteMelodyFactory>();
            clone.Root = Root.Clone() as ScriptableNotePool;
            clone.Scale = Scale.Clone() as ScriptableScalePool;
            clone.Octave = Octave.Clone() as RandomIntReference;
            return clone;
        }

        protected override MelodyFactory GetFactory()
        {
            return new BreakNoteMelodyFactory(
                Root.GetNext(),
                Scale.GetNext(),
                Octave,
                TimeSignature,
                NumberOfBreaks
            );
        }
    }
}