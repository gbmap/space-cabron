using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Trauma;
    public float ShakeFactor
    {
        get { return Mathf.Pow(Trauma, 2f); }
    }
    
    // Update is called once per frame
    void Update()
    {
        Trauma = Mathf.Clamp01(Trauma - Time.deltaTime);

        float t = Time.time * 100f;

        float r1 = (Mathf.PerlinNoise(t+10, t)-0.5f)*2f;
        float r2 = (Mathf.PerlinNoise(t+20, t) - 0.5f)*2f;
        float r3 = (Mathf.PerlinNoise(t+30, t) - 0.5f)*2f;

        float a = 10f * ShakeFactor * r1;
        Vector3 offset = new Vector3(
            0.1f * ShakeFactor * r2,
            0.1f * ShakeFactor * r3,
            0f);

        transform.localPosition = offset;
        transform.localRotation = Quaternion.Euler(0f, 0f, a);
    }
}
