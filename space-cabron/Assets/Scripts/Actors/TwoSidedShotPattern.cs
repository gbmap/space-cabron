using System.Collections;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class TwoSidedShotPattern : ShotPattern
    {
        public float Angle = 15f;
        public int NumberOfShots = 5;
        public float DelayInSeconds = 0.5f;
        public override IEnumerator ShootCoroutine()
        {
            Quaternion rotA = Quaternion.Euler(0f, 0f, 180f+transform.localRotation.eulerAngles.z + Angle);
            Quaternion rotB = Quaternion.Euler(0f, 0f, 180f+transform.localRotation.eulerAngles.z - Angle);

            for (int i = 0; i < NumberOfShots; i++)
            {
                SinMovement sM2 = Shoot(transform.position, rotB).GetComponent<SinMovement>();
                if (sM2)
                {
                    sM2.Amplitude = Mathf.Abs(sM2.Amplitude);
                }

                SinMovement sM = Shoot(transform.position, rotA).GetComponent<SinMovement>(); 
                if (sM)
                {
                    sM.Amplitude=Mathf.Abs(sM.Amplitude)*-1f;
                    // sM.UseCos=true;
                }
                yield return new WaitForSeconds(DelayInSeconds);
            }
        }
    }
}