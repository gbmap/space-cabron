using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap.Instruments
{
    public class AddImprovisationOnScoreModulus : OnScoreModulusBehaviour
    {
        public ImprovisationPool improvisationPool;
        public bool Permanent;
        public Counter improvisationCounter;

        protected override void HandleEvent()
        {
            if (turntable == null || improvisationPool == null)
                return;

            var improvisation = improvisationPool.GetNext().Get();
            turntable.ApplyImprovisation(improvisation, Permanent);

            if (improvisationCounter.Increase())
                Destroy(this);
        }
    }
}