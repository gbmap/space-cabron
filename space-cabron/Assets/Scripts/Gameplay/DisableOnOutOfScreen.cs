using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnOutOfScreen : MonoBehaviour
{
    public System.Action OnOutOfScreen;

    void Update()
    {
        /*Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x > 1.1f || pos.x < -0.1f || pos.y > 1.1f || pos.y < -0.1f)
        {
            this.DestroyOrDisable();
        }*/

        Vector2 pos = transform.position;
        if (Camera.main != null)
        {
            Vector2 cPos = Camera.main.transform.position;
            pos = pos - cPos;
        }

        if (pos.sqrMagnitude > 35f)
        {
            OnOutOfScreen?.Invoke();
            this.DestroyOrDisable();
        }
    }
}
