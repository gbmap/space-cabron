using System.Collections;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Gun;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceCabron.Gameplay
{ 
    [System.Serializable]
    public class TurntableResolver
    {
        // Gets ChildObjectName from the first object found with Tag. 
        public string Tag;
        public string ChildObjectName;
        public virtual TurntableBehaviour Get()
        {
            GameObject obj = GameObject.FindGameObjectWithTag(Tag);
            if (obj == null) {
                Debug.LogWarning($"Object with tag {Tag} not found.");
                return null;
            }
            
            Transform transform = obj.transform.Find(ChildObjectName);
            if (transform == null) {
                return null;
                // throw new System.Exception("Couldn't find child object " + ChildObjectName + " on object with tag " + Tag);
            }

            TurntableBehaviour tb = transform.GetComponentInChildren<TurntableBehaviour>();
            return tb;
        }

        public static TurntableResolver Create(string tag, string childObjectName) {
            return new TurntableResolver {
                Tag = tag,
                ChildObjectName = childObjectName
            };
        }
    }

    public class Fire : MonoBehaviour, IBrainHolder<InputState>
    {
       
        public bool UseTurntableResolver = false;
        public TurntableResolver Resolver;

        public Gmap.ScriptableReferences.FloatReference EnergyValue;

        public IBrain<InputState> Brain { get; set; }
        public float LastNote { get; private set; }

        float _lastPress = 0f;
        bool _shouldFire;
        bool _canFire = true;
        bool _isSpecial = false;
        float _waitTime = 0.125f;
        float WaitTime {
            get {
                return _waitTime;
            }
        }

        float _energy;
        float Energy
        {
            get { return _energy = 1f; }
            set 
            { 
                float v = Mathf.Clamp01(value);
                if (EnergyValue != null)
                    EnergyValue.Value = v;
                _energy = v;
            }
        }
        float _energyLoss = 0.1f;

        public OnNoteArgs LastNoteArgs;
        protected ShotData lastShotData = new ShotData{};
        protected InputState LastInputState = new InputState{};

        TurntableBehaviour turntable;
        GunBehaviour gun;

        public UnityEvent<OnNoteArgs> OnFire;

        protected virtual void Awake()
        {
            gun = GetComponentInChildren<GunBehaviour>();
        }

        void Start()
        {
            turntable = GetTurntable(UseTurntableResolver);
            if (turntable) {
                turntable.UnityEvent.AddListener(Callback_OnNote);
            }
        }

        protected TurntableBehaviour GetTurntable(bool useResolver)
        {
            if (useResolver)
                return Resolver.Get();
            else
                return GetComponentInChildren<TurntableBehaviour>();
        }

        public void Callback_OnNote(OnNoteArgs n)
        {
            if (!_canFire)
                return;

            if (LastNoteArgs == null) {
                LastNoteArgs = n;
            }

            bool special = Mathf.Abs(Time.time - _lastPress) < WaitTime;
            LastNoteArgs = n;
            if (special)
            {
                Energy += _energyLoss * 1f/3f;
                FireGun(n, true);
            }
            else 
            {
                waitingForPress = StartCoroutine(WaitForPress());
            }
            LastNote = Time.time;
        }

        protected virtual void Update()
        {
            if (Brain == null)
                return;

            LastInputState = Brain.GetInputState(new InputStateArgs{Object=gameObject});
            if (LastInputState.Shoot && LastNoteArgs != null) {
                bool wrongTime = Time.time - (LastNote + LastNoteArgs.Duration - WaitTime) < 0f;
                if (waitingForPress == null && wrongTime) {
                    MessageRouter.RaiseMessage(new MsgOnNotePlayedOutOfTime {
                        PlayerIndex = Brain is ScriptableInputBrain 
                                    ? ((ScriptableInputBrain)Brain).Index 
                                    : -1
                    });
                }
                _lastPress = Time.time;
            }
            Energy = Mathf.Clamp01(Energy + Time.deltaTime*0.1f);
        }

        Coroutine waitingForPress;
        public bool WaitingForPress { get { return waitingForPress != null;} }
        IEnumerator WaitForPress()
        {
            if (Brain == null)
                yield break;

            float timeWaited = 0f;
            while (timeWaited < WaitTime)
            {
                timeWaited += Time.deltaTime;
                if (LastInputState.Shoot)
                {
                    FireGun(LastNoteArgs, true);
                    waitingForPress = null;
                    yield break;
                }
                yield return null;
            }
            waitingForPress = null;
        }

        IEnumerator DisableGun(float time)
        {
            _canFire = false;
            yield return new WaitForSeconds(time);
            _canFire = true;
        }

        protected virtual void FireGun(OnNoteArgs args, bool special)
        {
            MessageRouter.RaiseMessage(new Messages.MsgOnNotePlayedInTime {
                PlayerIndex = gameObject.name[gameObject.name.Length-1] - '0'
            });

            _isSpecial = special;
            lastShotData = gun.Fire(new FireRequest(
                Mathf.Max(0.01f, args.Duration*5f),
                special,
                this 
            ));

            foreach (var instance in lastShotData.BulletInstances)
            {
                Bullet bullet = instance.GetComponent<Bullet>();
                if (bullet != null) {
                    bullet.ShotData = lastShotData;
                    bullet.IsSpecial = special;
                }
            }

            if (waitingForPress != null)
                StopCoroutine(waitingForPress);
            waitingForPress = null;
            _lastPress = -float.NegativeInfinity;

            OnFire?.Invoke(args);
        }
    }
}