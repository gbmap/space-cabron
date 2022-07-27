using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Instruments;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class InjectTurntableImprovisersOnAwake : Injectable
    {
        public ScriptableImprovisation[] Improvisations;
        void Awake()
        {
            var turntable = GetComponent<ITurntable>();
            System.Array.ForEach(Improvisations, x=> turntable.Improviser.AddImprovisation(x.Get()));
            Destroy(this);
        }
    }
}