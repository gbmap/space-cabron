using Managers;
using System.Collections;
using UnityEngine;

public abstract class ShotPattern : MonoBehaviour
{
    public GameObject Bullet;
    public float Cooldown = 2f;
    float _lastShot;

    static ObjectPool.GameObjectPool _enemyBulletPool;

    Animator _anim;
    int _animShotId;

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
        if (Time.time > _lastShot + Cooldown)
        {
            StartCoroutine(ShootCoroutine());
        }
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
