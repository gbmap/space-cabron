using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
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
        public IntReference N;

        public override SelectionStrategy Get()
        {
            return new EveryNStrategy(N.Value);
        }
    }
}