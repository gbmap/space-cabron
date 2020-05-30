using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D _rbody;

    // ==== ANIMATION CONTROL
    Animator _animator;
    int _animatorXHash;
    
    // ===== MOVEMENT CONTROL
    public float Speed;

    // ===== SHOOTING CFG
    [Header("Shooting Config")]
    public GameObject Bullet;
    ObjectPool.GameObjectPool _pool;

    EInstrumentAudio[] _shotPattern;
    int _currentBeat;
    
    public AudioClip ShotSound;

    public BeatMaker Turntable;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _animatorXHash = Animator.StringToHash("X");

        _pool = new ObjectPool.GameObjectPool(Bullet);
        _pool.InitPool(200);

        ShuffleShotPattern();
    }

    private void ShuffleShotPattern()
    {
        if (_shotPattern == null)
        {
            _shotPattern = new EInstrumentAudio[8];
        }

        for (int i = 0; i < _shotPattern.Length; i++)
        {
            _shotPattern[i] = UnityEngine.Random.Range(0f, 1f) > 0.75f ? EInstrumentAudio.None : EInstrumentAudio.Kick;
        }
    }

    private void OnEnable()
    {
        Turntable.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        Turntable.OnBeat -= OnBeat;
    }

    private void OnBeat(int[] obj)
    {
        _currentBeat = (++_currentBeat) % _shotPattern.Length;

        if (_shotPattern[_currentBeat] != EInstrumentAudio.None)
        {
            _pool.Instantiate(transform.position + transform.right*0.1f, Quaternion.identity);
            _pool.Instantiate(transform.position - transform.right*0.1f, Quaternion.identity);
            AudioSource.PlayClipAtPoint(ShotSound, transform.position);
        }
    }

    private void FixedUpdate()
    {
        Vector2 input = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
        _animator.SetFloat(_animatorXHash, input.x);
        _rbody.MovePosition(_rbody.position + input * Speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("IncreaseBPM"))
        {
            Turntable.BPM += 5;
            Destroy(collision.gameObject);
        }
    }
}
