﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerShot : ShotPattern
{
    public int Bullets = 10;
    public float CooldownPerBullet = 0.05f;
    public float AngleRange = 20f;
    public float AngleOffset = 0f;
    public bool RandomizeAngleOffset;
    public bool FollowPlayer;

    public override IEnumerator ShootCoroutine()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < Bullets; i++)
        {
            float a = GetAngle(player);
            float r = (Mathf.PerlinNoise(0f, Time.time*10f)-0.5f)*AngleRange;
            Shoot(transform.position, Quaternion.Euler(0f, 0f, a+r));
            yield return new WaitForSeconds(CooldownPerBullet);
        }
    }

    protected virtual float GetAngle(GameObject player)
    {
        float a = AngleOffset + Vector3.SignedAngle(Vector3.up, transform.up, Vector3.forward);
        if (FollowPlayer && player != null) {
            a = Vector3.SignedAngle(Vector3.up, player.transform.position - transform.position, Vector3.forward);
        }
        if (RandomizeAngleOffset)
            a += (float)(new System.Random((int)Time.time).NextDouble()-0.5f)*AngleRange;
        return a;
    }
}