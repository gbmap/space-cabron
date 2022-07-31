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
        public override Melody Generate()
        {
            return new StringPoolMelodyFactory(PossibleMelodies).Generate();
        }
    }
}