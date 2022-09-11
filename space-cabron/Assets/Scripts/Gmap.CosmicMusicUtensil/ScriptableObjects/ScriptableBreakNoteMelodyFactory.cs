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
        public Vector2IntPool TimeSignature;
        public ImprovisationPool ImprovisationPool;
        public int NumberOfBreaks = 3;
        public int NumberOfBars = 2;

        public override ScriptableMelodyFactory Clone()
        {
            var clone = ScriptableObject.CreateInstance<ScriptableBreakNoteMelodyFactory>();
            clone.Root = Root.Clone() as ScriptableNotePool;
            clone.Scale = Scale.Clone() as ScriptableScalePool;
            clone.Octave = Octave.Clone() as RandomIntReference;
            clone.TimeSignature = TimeSignature.Clone() as Vector2IntPool;
            clone.NumberOfBreaks = NumberOfBreaks;
            clone.ImprovisationPool = ImprovisationPool.Clone() as ImprovisationPool;
            return clone;
        }

        protected override MelodyFactory GetFactory()
        {
            return new RandomImprovisationMelodyFactory(
                Root.GetNext(), 
                Scale.GetNext(), 
                Octave, 
                TimeSignature.GetNext(), 
                NumberOfBreaks, 
                ImprovisationPool,
                NumberOfBars
            );
        }
    }
}