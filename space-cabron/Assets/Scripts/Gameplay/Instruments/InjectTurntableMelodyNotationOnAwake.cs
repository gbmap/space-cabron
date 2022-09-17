using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            IMelodyPlayer[] melodyPlayers = GetComponentsInChildren<IMelodyPlayer>();
            IMelodyPlayer mp = melodyPlayers.OrderByDescending(x=>x.MelodyPlayerPriority).FirstOrDefault();
            mp.Generate(MelodyFactory);
            Destroy(this);

            // MelodySwitcher ms = GetComponent<MelodySwitcher>();
            // if (ms != null) {
            //     ms.GenerateMelodies(MelodyFactory);
            // }
            // else {
            //     ITurntable behaviour = GetComponent<ITurntable>();
            //     Melody m = MelodyFactory.GenerateMelody();
            //     behaviour.SetMelody(m);
            //     Destroy(this);
            // }
        }
    }
}