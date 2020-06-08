using Frictionless;
using ObjectPool;
using UnityEngine;

public class Health : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
{
    public int MaxHealth;
    int _currentHealth;
    
    /* VISUALS */
    SpriteRenderer _spriteRenderer;
    int _damageId = Shader.PropertyToID("_Damage");

    MessageRouter _messageRouter;
    ObjectPool.ObjectPoolBehavior _poolBehavior;

    public System.Action<Health> OnDestroy;

    void Awake()
    {
        _poolBehavior = GetComponent<ObjectPool.ObjectPoolBehavior>();
        _currentHealth = MaxHealth;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _messageRouter = ServiceFactory.Instance.Resolve<MessageRouter>();
    }

    void Update()
    {
        float _damage = _spriteRenderer.material.GetFloat(_damageId);
        _spriteRenderer.material.SetFloat(_damageId, Mathf.Lerp(_damage, 0f, Time.deltaTime*2f));
    }

    public void TakeDamage(Bullet b, Collider2D collider)
    {
        _currentHealth--;
        _spriteRenderer.material.SetFloat(_damageId, 1.0f);

        if (_currentHealth == 0)
        {
            //FX.Instance.SpawnExplosionCluster(MaxHealth, transform.position);

            OnDestroy?.Invoke(this);

            _messageRouter.RaiseMessage(new MsgOnEnemyDestroyed()
            {
                enemy = this,
                bullet = b,
                collider = collider
            });
        }

        _messageRouter.RaiseMessage(new MsgOnEnemyHit()
        {
            enemy = this,
            bullet = b,
            collider = collider
        });

        if (_currentHealth == 0)
            this.DestroyOrDisable();
    }

    void IObjectPoolEventHandler.PoolReset()
    {
        OnDestroy?.Invoke(this);
        _currentHealth = MaxHealth;
    }
}
