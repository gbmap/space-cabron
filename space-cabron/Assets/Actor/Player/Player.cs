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
    public Synth Instrument;

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

    private void OnNote(ENote obj)
    {
        _pool.Instantiate(transform.position + transform.right * 0.1f, Quaternion.identity);
        _pool.Instantiate(transform.position - transform.right * 0.1f, Quaternion.identity);
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

    private void OnDisable()
    {
        Instrument.OnNote -= OnNote;
    }

    private void FixedUpdate()
    {
        Vector2 input = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
        _animator.SetFloat(_animatorXHash, input.x);
        _rbody.MovePosition(_rbody.position + input * Speed);
    }
}
