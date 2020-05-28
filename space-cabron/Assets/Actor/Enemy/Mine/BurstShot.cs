using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstShot : ShotPattern
{
    public float AngleIncrement = 30f;

    public override IEnumerator ShootCoroutine()
    {
        for (float a = 0f; a < 360f; a += AngleIncrement)
        {
            Shoot(transform.position, Quaternion.Euler(0f, 0f, a));
        }

        yield break;
    }
}
