using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnOutOfScreen : MonoBehaviour
{
    void Update()
    {
        /*Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x > 1.1f || pos.x < -0.1f || pos.y > 1.1f || pos.y < -0.1f)
        {
            this.DestroyOrDisable();
        }*/

        if (transform.position.sqrMagnitude > 25f)
        {
            this.DestroyOrDisable();
        }
    }
}
