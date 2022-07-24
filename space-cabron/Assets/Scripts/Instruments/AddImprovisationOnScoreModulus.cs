using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Instruments
{
    public class AddImprovisationOnScoreModulus : OnScoreModulusBehaviour
    {
        public ImprovisationPool improvisationPool;

        protected override void HandleEvent()
        {
            if (turntable == null)
                return;

            turntable.Improviser.AddImprovisation(
                improvisationPool.GetNext().Get()
            );
        }
    }
}