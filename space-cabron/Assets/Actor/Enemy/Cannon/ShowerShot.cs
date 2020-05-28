using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerShot : ShotPattern
{
    public int Bullets = 10;
    public float CooldownPerBullet = 0.05f;
    public float AngleRange = 20f;

    public override IEnumerator ShootCoroutine()
    {
        for (int i = 0; i < Bullets; i++)
        {
            Shoot(transform.position, Quaternion.Euler(0f, 0f, 180f+ (Mathf.PerlinNoise(0f, Time.time*10f)-0.5f)*AngleRange) );
            yield return new WaitForSeconds(CooldownPerBullet);
        }
    }
}
