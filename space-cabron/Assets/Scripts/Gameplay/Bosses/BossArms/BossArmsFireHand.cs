using System.Collections;
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
        // StartCoroutine(FireHand(GameObject.FindGameObjectWithTag("Player")));
    }

    public IEnumerator FireHand(GameObject player)
    {
        float time = 0f;
        int numberOfIterations = Random.Range(100, 200);
        float targetTime = Random.Range(5f, 7f);
        while (time <= targetTime)
        {
            if (player == null)
            {
                shoulder.transform.Rotate(
                    Vector3.forward * Mathf.Sin(time*RotationSpeed + Mathf.PI / 2f) * AngleRange * Time.deltaTime, 
                    Space.Self
                );
            }
            else
            {
                Vector3 deltaPlayer = player.transform.position - shoulder.transform.position;
                Vector3 deltaHand = hand.transform.position - shoulder.transform.position;
                float angle = Vector3.SignedAngle(deltaPlayer, deltaHand, Vector3.forward);
                shoulder.transform.Rotate(
                    Vector3.forward * -angle * RotationSpeed * Time.deltaTime
                );
            }
            yield return null;
            time += Time.deltaTime;
        }

        yield return new WaitForSeconds(1.0f);

        while (Camera.main.WorldToViewportPoint(hand.position).y > 0.05f)
        {
            hand.transform.localPosition += Vector3.down * HandSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator RecoverHand()
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
