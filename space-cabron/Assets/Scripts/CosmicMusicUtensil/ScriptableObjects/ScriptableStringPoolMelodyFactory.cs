using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Melody Factory/String Pool")]
    public class ScriptableStringPoolMelodyFactory : ScriptableMelodyFactory
    {
        public StringReferencePool PossibleMelodies;

        public override Melody GenerateMelody()
        {
            return new StringPoolMelodyFactory(PossibleMelodies).GenerateMelody();
        }

        public override ScriptableMelodyFactory Clone()
        {
            var instance = ScriptableObject.CreateInstance<ScriptableStringPoolMelodyFactory>();
            instance.PossibleMelodies = PossibleMelodies.Clone() as StringReferencePool;
            return instance;
        }

    }
}