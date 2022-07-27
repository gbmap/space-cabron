using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class ScriptableSelectionStrategy : ScriptableObject
    {
        public abstract SelectionStrategy Get();
    }

    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Selection Strategies/Every N Selection")]
    public class ScriptableEveryNSelection : ScriptableSelectionStrategy
    {
        public int N;

        public override SelectionStrategy Get()
        {
            return new EveryNStrategy(N);
        }
    }

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