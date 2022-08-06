using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class ScriptableMelodyFactory : ScriptableObject, MelodyFactory, ICloneable<ScriptableMelodyFactory>
    {
        public abstract ScriptableMelodyFactory Clone();
        public abstract Melody GenerateMelody();
    }
}