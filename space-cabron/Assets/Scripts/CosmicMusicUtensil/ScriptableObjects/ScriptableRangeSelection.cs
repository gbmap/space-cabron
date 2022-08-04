using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Selection Strategies/Range Selection")]
    public class ScriptableRangeSelection : ScriptableSelectionStrategy
    {
        public Vector2Int MinMax;

        public override SelectionStrategy Get()
        {
            return new RangeStrategy(MinMax.x, MinMax.y);
        }
    }
}
