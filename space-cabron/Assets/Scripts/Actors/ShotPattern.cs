using Gmap.Gameplay;
using Managers;
using SpaceCabron.Gameplay;
using System.Collections;
using UnityEngine;

public abstract class ShotPattern : MonoBehaviour, IBrainHolder<InputState>
{
    public GameObject Bullet;
    public float Cooldown = 2f;
    float _lastShot;

    static ObjectPool.GameObjectPool _enemyBulletPool;

    Animator _anim;
    int _animShotId;

    public bool CanFire
    {
        get { return Time.time > _lastShot + Cooldown; }
    }

    private IBrain<InputState> Brain;
    IBrain<InputState> IBrainHolder<InputState>.Brain 
    { 
        get => Brain; 
        set => Brain = value; 
    }

    public abstract IEnumerator ShootCoroutine();

    void Awake()
    {
        if (_enemyBulletPool == null)
        {
            _enemyBulletPool = new ObjectPool.GameObjectPool(Bullet);
            _enemyBulletPool.InitPool(500);
        }

        _anim = GetComponent<Animator>();
        _animShotId = Animator.StringToHash("Shot");
    }

    private void Update()
    {
        if (CanFire && Brain.GetInputState(new InputStateArgs {Object=gameObject}).Shoot)
            StartCoroutine(ShootCoroutine());
    }

    public void Fire()
    {
        StartCoroutine(ShootCoroutine());
    }

    public GameObject Shoot(Vector3 pos, Quaternion rot)
    {
        _lastShot = Time.time;

        if (_anim)
        {
            _anim.SetTrigger(_animShotId);
        }

        return _enemyBulletPool.Instantiate(pos, rot);
    }
}
