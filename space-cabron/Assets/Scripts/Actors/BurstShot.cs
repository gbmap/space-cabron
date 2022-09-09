using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

public class BurstShot : ShotPattern
{
    public float AngleIncrement = 30f;
    public float TimeBetweenBullets = 0f;
    public bool Relative = true;
    public FloatBusReference AngleOffset;

    public override IEnumerator ShootCoroutine()
    {
        for (float a = 0f; a < 360f; a += AngleIncrement)
        {
            float fa = Relative?transform.localRotation.z:0f
                     + AngleOffset.Value;
            Shoot(transform.position, Quaternion.Euler(0f, 0f, fa+a));
            yield return new WaitForSeconds(TimeBetweenBullets);
        }

        yield break;
    }
}
