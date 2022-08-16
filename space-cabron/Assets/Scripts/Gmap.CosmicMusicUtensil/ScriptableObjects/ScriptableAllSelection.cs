using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Selection Strategies/Select All")]
    public class ScriptableAllSelection : ScriptableSelectionStrategy
    {
        public override SelectionStrategy Get()
        {
            return new SelectAllStrategy();
        }
    }
}