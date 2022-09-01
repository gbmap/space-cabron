using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class DiscreteRandomizeRotation : MonoBehaviour
    {
        public bool BothSigns = true;
        public float Angle = 90f;
        public float Interval = 2f;

        private float lastRandomization;

        void Awake()
        {
            lastRandomization = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - lastRandomization >= Interval)
            {
                RandomizeRotation();
                lastRandomization = Time.time;
            }
        }

        private void RandomizeRotation()
        {
            float angle = Angle;
            if (BothSigns)
                angle = UnityEngine.Random.value > 0.5 ? angle : -angle;

            transform.localRotation *= Quaternion.Euler(
                0f, 
                0f, 
                angle
            );
        }
    }
}