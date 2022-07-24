using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Selection Strategies/Random")]
    public class ScriptableRandomSelection : ScriptableSelectionStrategy
    {
        public override SelectionStrategy Get()
        {
            return new RandomSelectionStrategy();
        }
    }

}