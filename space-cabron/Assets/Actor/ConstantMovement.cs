using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ConstantMovement : MonoBehaviour
{
    public Vector2 Direction = new Vector2(0f, -1f);
    public float Rotation = 0f;
    public float Speed = 0.03f;

    public bool Absolute = false;

    Rigidbody2D _rbody;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Absolute)
        {
            _rbody.MovePosition(_rbody.position + Direction * Speed);
            _rbody.SetRotation(_rbody.rotation+Rotation*Time.deltaTime);
        }
        else
        {
            Vector2 v = ((transform.up * Direction.y) + (transform.right * Direction.x));
            _rbody.MovePosition(_rbody.position + v * Speed);
            _rbody.MoveRotation(_rbody.rotation + Rotation * Time.deltaTime);
        }
    }
}
