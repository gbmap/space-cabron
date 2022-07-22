using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.CosmicMusicUtensil;

namespace SpaceCabron
{
    public class IncreaseTurntableBPMOnCollision : MonoBehaviour
    {
        public int BPMIncrease = 10;

        void OnTriggerEnter2D(Collider2D other)
        {
            var turntable = other.GetComponentInChildren<TurntableBehaviour>();
            if (turntable != null)
                turntable.SetBPM(turntable.BPMReference.Value + BPMIncrease);
        }
    }
}