using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowMovement : MonoBehaviour
{
    public float Speed;

    Rigidbody2D rigidbody;
    GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
            player = players[Random.Range(0, players.Length)];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 delta = (player.transform.position - transform.position);
        rigidbody.MovePosition(rigidbody.position + delta.normalized * Speed * Time.deltaTime);
    }
}
