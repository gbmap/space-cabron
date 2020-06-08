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

    public NoteSequencer Instrument;

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
        Instrument.OnNote += OnNote;
    }
    
    private void OnDisable()
    {
        Instrument.OnNote -= OnNote;
    }

    private void OnNote(ENote[] note)
    {
        if (note[0] == ENote.None) return;
        _pool.Instantiate(transform.position + transform.right * 0.1f, Quaternion.identity);
        _pool.Instantiate(transform.position - transform.right * 0.1f, Quaternion.identity);
    }


    private void FixedUpdate()
    {
        Vector2 input = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
        _animator.SetFloat(_animatorXHash, input.x);
        _rbody.MovePosition(_rbody.position + input * Speed);
    }
}
