using QFSW.QC;
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

    public Synth Synth;

    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _animatorXHash = Animator.StringToHash("X");

        _pool = new ObjectPool.GameObjectPool(Bullet);
        _pool.InitPool(200);
    }

    private void Start()
    {
        // Synth.OnNotePlayed += OnNote;

    }
    
    private void OnDisable()
    {
        // Synth.OnNotePlayed -= OnNote;
    }

    private void OnNote(ENote note)
    {
        if (note == ENote.None) return;
        _pool.Instantiate(transform.position + transform.right * 0.1f, Quaternion.identity);
        _pool.Instantiate(transform.position - transform.right * 0.1f, Quaternion.identity);
    }

    private void FixedUpdate()
    {
        Vector2 input = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
        _animator.SetFloat(_animatorXHash, input.x);
        if (!QuantumConsole.Instance.IsActive)
        { 
            _rbody.MovePosition(_rbody.position + input * Speed);
        }
    }
}
