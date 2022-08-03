using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace Gmap.Instruments
{
    public class AddImprovisationOnBarModulus : MonoBehaviour
    {
        public int BarModulus = 4;
        public ImprovisationPool improvisationPool;

        protected ITurntable turntable;

        private int lastBarIndex = 0;

        void Awake()
        {
            turntable = GetComponent<ITurntable>();
        }

        void FixedUpdate()
        {
            if (lastBarIndex == turntable.BarIndex)
                return;


            int barModulus = Mathf.Max(1, Mathf.FloorToInt(turntable.BPM / 60))*BarModulus;

            if (turntable.BarIndex % BarModulus == 0)
            {
                var improvisation = improvisationPool.GetNext().Get();
                UnityEngine.Debug.Log("Adding\n" + improvisation.ToString());
                turntable.SetMelody(
                    turntable.Melody.ApplyImprovisation(improvisation)
                );
            }

            lastBarIndex = turntable.BarIndex;
        }
    }
}