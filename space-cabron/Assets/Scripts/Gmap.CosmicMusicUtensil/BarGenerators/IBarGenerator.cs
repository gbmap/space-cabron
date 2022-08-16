using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface MelodyGenerator
    {
        Note GetNote(int i);
    }

    public interface BarGenerator 
    {
    }
}