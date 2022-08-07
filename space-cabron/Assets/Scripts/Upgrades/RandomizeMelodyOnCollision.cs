using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class RandomizeMelodyOnCollision : CollisionHandler<TurntableBehaviour>
    {
        public ScriptableMelodyFactory MelodyFactory; 

        protected override void HandleCollision(TurntableBehaviour turntable)
        {
            turntable.SetMelody(MelodyFactory.GenerateMelody());
        }
    }
}