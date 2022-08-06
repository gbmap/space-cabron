using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap.Instruments
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class InjectTurntableMelodyNotationOnAwake : Injectable
    {
        public ScriptableMelodyFactory MelodyFactory;

        void Start()
        {
            ITurntable behaviour = GetComponent<ITurntable>();
            Melody m = MelodyFactory.GenerateMelody();
            behaviour.SetMelody(m);
            Destroy(this);
        }
    }
}