using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public abstract class ScriptableMelodyFactory : ScriptableObject, MelodyFactory, ICloneable<ScriptableMelodyFactory>
    {
        public MelodyFactory LastUsedFactory { get; private set; }
        public abstract ScriptableMelodyFactory Clone();

        public Melody GenerateMelody()
        {
            LastUsedFactory = GetFactory();
            return LastUsedFactory.GenerateMelody();
        }

        protected abstract MelodyFactory GetFactory();
    }
}