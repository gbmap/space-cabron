using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnOutOfScreen : MonoBehaviour
{
    ObjectPool.ObjectPoolBehavior _poolBehavior;
    // Start is called before the first frame update
    void Start()
    {
        _poolBehavior = GetComponent<ObjectPool.ObjectPoolBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x > 1.1f || pos.x < -0.1f || pos.y > 1.1f || pos.y < -0.1f)
        {
            if (_poolBehavior)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
