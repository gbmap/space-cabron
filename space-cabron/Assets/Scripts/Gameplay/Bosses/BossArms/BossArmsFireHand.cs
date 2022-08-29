using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArmsFireHand : MonoBehaviour
{
    [SerializeField] Transform shoulder;
    [SerializeField] Transform hand;

    public float RotationSpeed = 5f;
    public float AngleRange = 2f;
    public float HandSpeed = 10f;

    private float initialHandLocalY;

    void Start()
    {
        initialHandLocalY = hand.transform.localPosition.y;
        StartCoroutine(FireHand());
    }

    IEnumerator FireHand()
    {
        while (true)
        {
            float time = 0f;
            int numberOfIterations = Random.Range(100, 200);
            float targetTime = Random.Range(5f, 7f);
            while (time <= targetTime)
            {
                shoulder.transform.Rotate(
                    Vector3.forward * Mathf.Sin(time*RotationSpeed + Mathf.PI / 2f) * AngleRange * Time.deltaTime, 
                    Space.Self
                );
                yield return null;
                time += Time.deltaTime;
            }

            yield return new WaitForSeconds(1.0f);

            while (Camera.main.WorldToViewportPoint(hand.position).y > 0.05f)
            {
                hand.transform.localPosition += Vector3.down * HandSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(5f);
            yield return RecoverHand();
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator RecoverHand()
    {
        while (Mathf.Abs(hand.transform.localPosition.y - initialHandLocalY) > 0.1f)
        {
            float delta = initialHandLocalY - hand.transform.localPosition.y;
            hand.transform.localPosition += Vector3.up 
                                         * HandSpeed*0.25f 
                                         * delta 
                                         * Time.deltaTime;
            yield return null;
        }

        while (Mathf.Abs(shoulder.transform.rotation.z) > 0.001f)
        {
            float z = shoulder.transform.rotation.z;
            shoulder.transform.rotation = Quaternion.Lerp(
                shoulder.transform.rotation, 
                Quaternion.identity, 
                Time.deltaTime
            );
            yield return null;
        }
        yield break;
    }
}
