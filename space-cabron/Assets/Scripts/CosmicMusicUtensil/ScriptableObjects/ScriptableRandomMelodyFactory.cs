using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class ScriptableFixedMelodyFactory : ScriptableMelodyFactory
    {
        public string Notation;

        public override ScriptableMelodyFactory Clone()
        {
            var instance = ScriptableObject.CreateInstance<ScriptableFixedMelodyFactory>();
            instance.Notation = Notation;
            return instance;
        }

        protected override MelodyFactory GetFactory()
        {
            return new FixedMelodyFactory(Notation);
        }
    }

    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody Factory/Random")]
    public class ScriptableRandomMelodyFactory : ScriptableMelodyFactory
    {
        public ScriptableNotePool Root;
        public ScriptableScalePool Scale;
        public RandomIntReference NumberOfNotes;
        public RandomIntReference Octave;
        public bool ConstantOctave;

        public override ScriptableMelodyFactory Clone()
        {
            var instance = ScriptableObject.CreateInstance<ScriptableRandomMelodyFactory>();
            instance.Root = Root.Clone() as ScriptableNotePool;
            instance.Scale = Scale.Clone() as ScriptableScalePool;
            instance.NumberOfNotes = NumberOfNotes.Clone() as RandomIntReference;
            instance.Octave = Octave.Clone() as RandomIntReference;
            instance.ConstantOctave = ConstantOctave;
            return instance;
        }

        protected override MelodyFactory GetFactory()
        {
            IntReference octaveRef = Octave;
            if (ConstantOctave)
            {
                octaveRef = ScriptableObject.CreateInstance<IntReference>();
                octaveRef.Value = Octave.Value;
            }

            return new RandomMelodyFactory(Root.GetNext(), Scale.GetNext(), NumberOfNotes.Value, octaveRef);
        }
    }
}