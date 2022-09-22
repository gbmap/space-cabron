using UnityEngine;

public class RotationFloatReference : MonoBehaviour
{
    public AnimationCurve RotationCurve;

    float t;

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        float angle = RotationCurve.Evaluate(t);
        transform.Rotate(Vector3.forward, angle*Time.deltaTime);
    }
}
