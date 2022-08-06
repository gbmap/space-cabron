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
    }
}