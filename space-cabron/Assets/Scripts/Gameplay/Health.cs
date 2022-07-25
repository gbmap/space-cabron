using Frictionless;
using Gmap.CosmicMusicUtensil;
using ObjectPool;
using UnityEngine;
using UnityEngine.Events;


namespace SpaceCabron.Gameplay
{
    public class MsgOnEnemyHit 
    {
        public string enemyName;
        public Bullet bullet;
        public Collider2D collider;
        public Health enemy;
    }
    public class MessageOnEnemyDestroyed : MsgOnEnemyHit { }

    public class Health : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
    {
        public int MaxHealth;
        int _currentHealth;
        
        /* VISUALS */
        SpriteRenderer _spriteRenderer;
        int _damageId = Shader.PropertyToID("_Damage");

        MessageRouter _messageRouter;
        ObjectPool.ObjectPoolBehavior _poolBehavior;

        public System.Action<MessageOnEnemyDestroyed> OnDestroy;
        public UnityEvent<OnNoteArgs> OnDamage;

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
                FireDestroyEvent(new MessageOnEnemyDestroyed
                {
                    enemyName = gameObject.name,
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
            _currentHealth = MaxHealth;
        }

        void FireDestroyEvent(MessageOnEnemyDestroyed msg, bool global=true)
        {
            OnDestroy?.Invoke(msg);
            _messageRouter.RaiseMessage(msg);
        }
    }
}