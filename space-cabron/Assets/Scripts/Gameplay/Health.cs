using Frictionless;
using Gmap.CosmicMusicUtensil;
using ObjectPool;
using UnityEngine;
using UnityEngine.Events;


namespace Gmap.Gameplay
{
    public class MsgOnObjectHit 
    {
        public string name;
        public Bullet bullet;
        public Collider2D collider;
        public Health health;
    }
    public class MsgOnObjectDestroyed : MsgOnObjectHit { }

    public class Health : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
    {
        public int MaxHealth;
        public bool CanTakeDamage = true;
        public bool IsResistant;

        int _currentHealth;
        public int CurrentHealth => _currentHealth;
        
        ObjectPool.ObjectPoolBehavior _poolBehavior;

        public System.Action<MsgOnObjectDestroyed> OnDestroy;
        public UnityEvent<OnNoteArgs> OnDamage;
        public System.Action OnTakenDamage;
        public System.Action<bool> OnSetResistant;

        void Awake()
        {
            _poolBehavior = GetComponent<ObjectPool.ObjectPoolBehavior>();
            _currentHealth = MaxHealth;

            DisableOnOutOfScreen d = GetComponent<DisableOnOutOfScreen>();
            if (d != null)
                d.OnOutOfScreen += Callback_OnOutOfScreen;

            SetIsResistant(IsResistant);
        }

        void Callback_OnOutOfScreen()
        {
            FireDestroyEvent(new MsgOnObjectDestroyed
            {
                bullet = null,
                collider = null,
                health = this,
                name = gameObject.name
            });
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


            // Bullet null means god doesn't like this creature.
            if (b == null || b.IsSpecial || !IsResistant)
            {
                _currentHealth--;
                OnTakenDamage?.Invoke();

                if (_currentHealth == 0)
                {
                    FireDestroyEvent(new MsgOnObjectDestroyed
                    {
                        name = gameObject.name,
                        health = this,
                        bullet = b,
                        collider = collider
                    });
                }

            MessageRouter.RaiseMessage(new MsgOnObjectHit()
            {
                health = this,
                bullet = b,
                collider = collider
            });

            if (_currentHealth == 0)
                this.DestroyOrDisable();
            }
        }

        void IObjectPoolEventHandler.PoolReset()
        {
            _currentHealth = MaxHealth;
        }

        void FireDestroyEvent(MsgOnObjectDestroyed msg, bool global=true)
        {
            OnDestroy?.Invoke(msg);
            MessageRouter.RaiseMessage(msg);
        }
    }
}