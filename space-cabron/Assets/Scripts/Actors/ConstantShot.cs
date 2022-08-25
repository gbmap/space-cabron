using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantShot : ShotPattern
{
    [SerializeField]
    private float _angle = 90f;

    [SerializeField]
    bool randomizeAngle;

    [SerializeField]
    float angleRange = 60f;
    
    public virtual float Angle { get { return _angle; } }
    

    public override IEnumerator ShootCoroutine()
    {
        float a = transform.localRotation.eulerAngles.z 
                + Angle
                + Random.Range(-angleRange, angleRange) 
                * (randomizeAngle ? 1f : 0);
        Shoot(transform.position, Quaternion.Euler(0f, 0f, a));
        yield return null;
    }
}
