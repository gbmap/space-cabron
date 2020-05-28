using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerMovement : MonoBehaviour
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float d = Vector3.Distance(transform.position, player.transform.position) * 2f;
        transform.position = Vector3.Lerp(transform.position, player.transform.position, Mathf.Pow(Mathf.Clamp01(1f - d), 2f));
    }
}
