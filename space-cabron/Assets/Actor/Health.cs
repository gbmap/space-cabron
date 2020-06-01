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

    public void TakeDamage()
    {
        _currentHealth--;
        
        _spriteRenderer.material.SetFloat(_damageId, 1.0f);

        if (_currentHealth == 0)
        {
            FX.Instance.SpawnExplosionCluster(MaxHealth, transform.position);

            OnDestroy?.Invoke(this);

            if (_poolBehavior)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }

            _messageRouter.RaiseMessage(new IncreaseScoreMessage() { Value = 100 });
            
        }
    }

    void IObjectPoolEventHandler.PoolReset()
    {
        _currentHealth = MaxHealth;
    }
}
