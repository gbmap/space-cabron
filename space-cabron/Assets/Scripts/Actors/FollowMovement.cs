using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowMovement : MonoBehaviour
{
    public float Speed;

    Rigidbody2D _rbody;
    GameObject _player;

    // Start is called before the first frame update
    void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 delta = (_player.transform.position - transform.position);
        _rbody.MovePosition(_rbody.position + delta.normalized * Speed * Time.deltaTime);
    }
}
