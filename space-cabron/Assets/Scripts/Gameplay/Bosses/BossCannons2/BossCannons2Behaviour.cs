using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses {
    public class BossCannons2Behaviour : BossBehaviour
    {
        public BossCannons2MainCannonBehaviour MainCannon;
        public BossCannons2CannonBehaviour[] Cannons;
        protected override IEnumerator CLogic()
        {
            // yield return StopFiringCannons(Cannons);

            while (true) {
                MainCannon.FireBigPingPong();
                yield return new WaitForSeconds(2.0f);

                int numberOfCannons = LerpByHealth(5, 5);
                numberOfCannons = Mathf.Clamp(numberOfCannons, 1, 4);

                var cannons = GetAvailableCannons(numberOfCannons);

                float[] angles = GetAngleForCannons(cannons.Length);
                yield return RotateCannonsTowards(cannons, angles);

                // yield return RotateCannonsAndFire(cannons);
                yield return StartFiringCannons(cannons);
                yield return new WaitForSeconds(2f);

                yield return MainCannon.FireRandomPattern();
                yield return new WaitForSeconds(2f);

                yield return StopFiringCannons(cannons);
                yield return new WaitForSeconds(2f);
            }
        }

        private IEnumerator RotateCannonsAndFire(BossCannons2CannonBehaviour[] cannons)
        {
            // BossCannons2CannonBehaviour[] cannons = GetAvailableCannons(numberOfCannons);
            // numberOfCannons = cannons.Length;

            
            yield return StartFiringCannons(cannons);
        }

        private IEnumerator FireCannons(BossCannons2CannonBehaviour[] cannons)
        {
            yield return StartFiringCannons(cannons);
            yield return new WaitForSeconds(1f);
            yield return StopFiringCannons(cannons);
        }

        private IEnumerator StartFiringCannons(BossCannons2CannonBehaviour[] cannons)
        {
            for (int i = 0; i < cannons.Length; i++) {
                if (cannons[i] == null) {
                    continue;
                }
                yield return cannons[i].Fire();
            }
        }

        private IEnumerator StopFiringCannons(BossCannons2CannonBehaviour[] cannons)
        {
            for (int i = 0; i < cannons.Length; i++) {
                if (cannons[i] == null) {
                    continue;
                }
                yield return cannons[i].StopFiring();
            }

        }

        private static float[] GetAngleForCannons(int numberOfCannons)
        {
            float[] angles = new float[numberOfCannons];

            // Get random angle for each cannon
            for (int i = 0; i < numberOfCannons; i++)
                angles[i] = UnityEngine.Random.Range(0, 360);
            return angles;
        }

        private IEnumerator RotateCannonsTowards(BossCannons2CannonBehaviour[] cannons, float[] angles)
        {
            // Rotate cannons to random angle.
            var cannonAngle = cannons.Zip(angles, (cannon, angle) => new { cannon, angle });
            while (cannonAngle.Any(c => Mathf.Abs(c.cannon.transform.rotation.eulerAngles.z - c.angle) > 0.1f))
            {
                for (int i = 0; i < cannons.Length; i++)
                {
                    RotateCannonTo(cannons[i], angles[i]);
                    yield return null;
                }
            }
        }


        private BossCannons2CannonBehaviour[] GetAvailableCannons(int numberOfCannons)
        {
            // Get available cannons
            IEnumerable<BossCannons2CannonBehaviour> cannons = Cannons.Where(c => c != null);
            if (cannons.Count() < numberOfCannons)
                return cannons.ToArray();
            return cannons.Take(numberOfCannons).ToArray();
        }

        private void RotateCannonTo(
            BossCannons2CannonBehaviour bossCannons2CannonBehaviour, 
            float angle
        ) {
            // Rotate cannon to angle
            bossCannons2CannonBehaviour.transform.rotation = Quaternion.RotateTowards(
                bossCannons2CannonBehaviour.transform.rotation,
                Quaternion.Euler(0, 0, angle),
                360 * Time.deltaTime
            );
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}