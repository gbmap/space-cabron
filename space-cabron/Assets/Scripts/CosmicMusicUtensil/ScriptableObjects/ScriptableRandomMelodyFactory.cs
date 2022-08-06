using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class ScriptableFixedMelodyFactory : ScriptableMelodyFactory
    {
        public string Notation;

        public override Melody GenerateMelody()
        {
            return new FixedMelodyFactory(Notation).GenerateMelody();
        }

        public override ScriptableMelodyFactory Clone()
        {
            var instance = ScriptableObject.CreateInstance<ScriptableFixedMelodyFactory>();
            instance.Notation = Notation;
            return instance;
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

        public override Melody GenerateMelody()
        {
            IntReference octaveRef = Octave;
            if (ConstantOctave)
            {
                octaveRef = ScriptableObject.CreateInstance<IntReference>();
                octaveRef.Value = Octave.Value;
            }

            return new RandomMelodyFactory(Root.GetNext(), Scale.GetNext(), NumberOfNotes.Value, octaveRef).GenerateMelody();
        }

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

    }
}