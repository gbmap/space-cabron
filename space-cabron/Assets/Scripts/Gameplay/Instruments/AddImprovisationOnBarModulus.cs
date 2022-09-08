using Gmap.CosmicMusicUtensil;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace Gmap.Instruments
{
    [System.Serializable]
    public class Counter
    {
        public int MaxCount = int.MaxValue;
        private int counter;

        public bool Increase()
        {
            return ++counter == MaxCount;
        }
    }
    
    public class AddImprovisationOnBarModulus : MonoBehaviour
    {
        public int BarModulus = 4;
        public bool Permanent;
        public Counter improvisationCounter;
        public FloatBusReference Probability;
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

            if (turntable.BarIndex % BarModulus == 0 && Random.value < Probability.Value)
            {
                var improvisation = improvisationPool.GetNext().Get();
                UnityEngine.Debug.Log("Adding\n" + improvisation.ToString());

                turntable.ApplyImprovisation(improvisation, Permanent);
                if (improvisationCounter.Increase())
                    Destroy(this);
            }

            lastBarIndex = turntable.BarIndex;
        }
    }
}