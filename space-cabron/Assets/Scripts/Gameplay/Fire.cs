using System.Collections;
using System.Collections.Generic;
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
        private class NoteQueueItem
        {
            public OnNoteArgs NoteArgs;
            public float Time;
        }
       
        public bool UseTurntableResolver = false;
        public TurntableResolver Resolver;

        public Gmap.ScriptableReferences.FloatReference EnergyValue;

        public IBrain<InputState> Brain { get; set; }
        public float LastNote { get; private set; }

        float _lastPress = 0f;
        bool _shouldFire;
        bool _canFire = true;
        bool _isSpecial = false;
        float _waitTime = 0.175f;
        float WaitTime {
            get {
                return _waitTime;
            }
        }

        public OnNoteArgs LastNoteArgs;
        protected ShotData lastShotData = new ShotData{};
        protected InputState LastInputState = new InputState{};

        TurntableBehaviour turntable;
        GunBehaviour gun;

        public UnityEvent<OnNoteArgs> OnFire;

        Queue<NoteQueueItem> _noteQueue = new Queue<NoteQueueItem>();

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
                FireGun(n, true);
            else {
                _noteQueue.Enqueue(new NoteQueueItem {
                    NoteArgs = n,
                    Time = Time.time
                });
            }
            // waitingForPress = StartCoroutine(WaitForPress());
            LastNote = Time.time;
        }

        protected virtual void Update()
        {
            if (Brain == null)
                return;

            LastInputState = Brain.GetInputState(new InputStateArgs{Object=gameObject});
            if (LastInputState.Shoot) {
                _lastPress = Time.time;
                if (LastNoteArgs != null) {
                if (_noteQueue.Count == 0 && (Time.time < (LastNote + LastNoteArgs.Duration - WaitTime))) {
                    MessageRouter.RaiseMessage(new MsgOnNotePlayedOutOfTime {
                        PlayerIndex = GetPlayerIndex() 
                    });
                }
                }
            }

            UpdateNoteQueue();
        }

        public bool WaitingForPress { get { return _noteQueue.Count > 0;} }
        
        void UpdateNoteQueue() {
            if (_noteQueue.Count == 0) {
                return;
            }

            var note = _noteQueue.Peek();
            if (Time.time > note.Time + WaitTime) {
                _noteQueue.Dequeue();
            }
            else if (LastInputState.Shoot) {
                FireGun(note.NoteArgs, true);
                _noteQueue.Dequeue();
            }
        }

        IEnumerator DisableGun(float time)
        {
            _canFire = false;
            yield return new WaitForSeconds(time);
            _canFire = true;
        }

        private int GetPlayerIndex() {
            if (Brain is ScriptableInputBrain) {
                return ((ScriptableInputBrain)Brain).Index;
            }
            return -1;
        }

        protected virtual void FireGun(OnNoteArgs args, bool special)
        {
            MessageRouter.RaiseMessage(new Messages.MsgOnNotePlayedInTime {
                PlayerIndex = GetPlayerIndex()
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

            _lastPress = -float.NegativeInfinity;

            OnFire?.Invoke(args);
        }
    }
}