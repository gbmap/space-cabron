using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class ScriptableFixedMelodyFactory : ScriptableMelodyFactory
    {
        public string Notation;
        public override Melody Generate()
        {
            return new FixedMelodyFactory(Notation).Generate();
        }
    }

    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody Factory/Random")]
    public class ScriptableRandomMelodyFactory : ScriptableMelodyFactory
    {
        public ScriptableNotePool Root;
        public ScriptableScalePool Scale;
        public RandomIntReference NumberOfNotes;

        public override Melody Generate()
        {
            return new RandomMelodyFactory(Root.GetNext(), Scale.GetNext(), NumberOfNotes.Value).Generate();
        }
    }
}