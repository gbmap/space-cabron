using Frictionless;
using Gmap.CosmicMusicUtensil;
using ObjectPool;
using UnityEngine;
using UnityEngine.Events;


namespace Gmap.Gameplay
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
        public bool CanTakeDamage = true;
        public bool IsResistant;

        int _currentHealth;
        public int CurrentHealth => _currentHealth;
        
        ObjectPool.ObjectPoolBehavior _poolBehavior;

        public System.Action<MessageOnEnemyDestroyed> OnDestroy;
        public UnityEvent<OnNoteArgs> OnDamage;
        public System.Action OnTakenDamage;
        public System.Action<bool> OnSetResistant;

        void Awake()
        {
            _poolBehavior = GetComponent<ObjectPool.ObjectPoolBehavior>();
            _currentHealth = MaxHealth;
        }

        void Start()
        {
            SetIsResistant(IsResistant);
        }


        public void SetIsResistant(bool v)
        {
            IsResistant = v;
            OnSetResistant?.Invoke(v);
        }

        public void TakeDamage(Bullet b, Collider2D collider)
        {
            if (!CanTakeDamage)
                return;

            if (b.IsSpecial || !IsResistant)
            {
                _currentHealth--;
                OnTakenDamage?.Invoke();

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
            }

                MessageRouter.RaiseMessage(new MsgOnEnemyHit()
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
            MessageRouter.RaiseMessage(msg);
        }
    }
}