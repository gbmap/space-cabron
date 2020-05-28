using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantShot : ShotPattern
{
    [SerializeField]
    private float _angle = 90f;
    
    public virtual float Angle { get { return _angle; } }
    

    public override IEnumerator ShootCoroutine()
    {
        Shoot(transform.position, Quaternion.Euler(0f, 0f, transform.localRotation.eulerAngles.z + Angle));
        yield return null;
    }
}
