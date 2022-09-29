using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public class RandomizeMelodyOnCollision : CollisionHandler
    {
        public ScriptableMelodyFactory MelodyFactory; 

        protected override void HandleCollision(TurntableBehaviour turntable)
        {
            MelodySwitcher ms = turntable.GetComponent<MelodySwitcher>();
            if (ms != null) {
                ms.Generate(MelodyFactory);
            }
            else {
                turntable.SetMelody(MelodyFactory.GenerateMelody());
            }
        }
    }
}