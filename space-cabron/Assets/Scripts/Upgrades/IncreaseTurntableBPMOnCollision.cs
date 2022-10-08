using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Gameplay;

namespace Gmap
{
    public class IncreaseTurntableBPMOnCollision : MonoBehaviour
    {
        public int BPMIncrease = 10;

        void OnTriggerEnter2D(Collider2D other)
        {
            var turntable = TurntableResolver.Create("GlobalInstruments", "PlayerInstrument").Get();
            if (turntable != null)
                turntable.SetBPM(turntable.BPMReference.Value + BPMIncrease);
        }
    }
}