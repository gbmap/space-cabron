using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gun;
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
        public MonoBehaviour objectFiring;
    }
    public class MsgOnObjectDestroyed : MsgOnObjectHit { }

    public class Health : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
    {
        public int MaxHealth;
        public bool CanTakeDamage = true;

        public bool IsBeingDestroyed { get; private set; }

        int _currentHealth;
        public int CurrentHealth => _currentHealth;
        
        ObjectPool.ObjectPoolBehavior _poolBehavior;

        public System.Action<MsgOnObjectDestroyed> OnDestroy;
        public UnityEvent<OnNoteArgs> OnDamage;
        public UnityEvent<MsgOnObjectDestroyed> OnDestroyUnity;
        public System.Action OnTakenDamage;
        public System.Action<bool> OnSetResistant;

        protected virtual void Awake()
        {
            _poolBehavior = GetComponent<ObjectPool.ObjectPoolBehavior>();
            _currentHealth = MaxHealth;

            DisableOnOutOfScreen d = GetComponent<DisableOnOutOfScreen>();
            if (d != null)
                d.OnOutOfScreen += Callback_OnOutOfScreen;
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

        public void SetIsResistant(bool v)
        {
            OnSetResistant?.Invoke(v);
        }
    
        public virtual bool TakeDamage(
            Bullet b, 
            Collider2D collider,
            MonoBehaviour objectFiring=null
        ) {
            if (!enabled)
                return false;

            if (!CanTakeDamage)
                return false;

            _currentHealth--;
            OnTakenDamage?.Invoke();

            if (_currentHealth == 0)
            {
                var msg = new MsgOnObjectDestroyed
                {
                    name = gameObject.name,
                    health = this,
                    bullet = b,
                    collider = collider
                };
                FireDestroyEvent(msg);
            }

            MessageRouter.RaiseMessage(new MsgOnObjectHit()
            {
                health = this,
                bullet = b,
                collider = collider,
                objectFiring = objectFiring
            });

            return true;
        }

        public void Destroy()
        {
            CanTakeDamage = true;
            _currentHealth = 1;
            TakeDamage(null, null);
        }


        void IObjectPoolEventHandler.PoolReset()
        {
            _currentHealth = MaxHealth;
        }

        void FireDestroyEvent(MsgOnObjectDestroyed msg, bool global=true)
        {
            IsBeingDestroyed = true;
            OnDestroy?.Invoke(msg);
            OnDestroyUnity?.Invoke(msg);
            MessageRouter.RaiseMessage(msg);
            this.DestroyOrDisable();
        }
    }
}