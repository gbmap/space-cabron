using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class TransposeMelodyImprovisation : TransposeNoteImprovisation
    {
        public TransposeMelodyImprovisation(SelectionStrategy barSelection, int steps)
            : base(new EveryNStrategy(1), barSelection, steps) {}
    }

}