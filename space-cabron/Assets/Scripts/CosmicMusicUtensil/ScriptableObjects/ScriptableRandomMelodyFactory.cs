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
        public ENote Root;
        public ScriptableScale Scale;
        public int NumberOfNotes = 4;

        public override Melody Generate()
        {
            return new RandomMelodyFactory(Root, Scale, NumberOfNotes).Generate();
        }
    }
}