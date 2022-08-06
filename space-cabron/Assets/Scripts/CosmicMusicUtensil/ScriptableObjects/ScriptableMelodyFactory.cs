using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class ScriptableMelodyFactory : ScriptableObject, MelodyFactory
    {
        public abstract Melody GenerateMelody();
    }
}