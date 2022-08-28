using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.Gameplay;
using Gmap.Gun;
using SpaceCabron.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class Fire : MonoBehaviour, IBrainHolder<InputState>
    {
        private class TurntableResolver
        {
            public virtual TurntableBehaviour Get(GameObject go)
            {
                return go.GetComponentInChildren<TurntableBehaviour>();
            }
        }

        public Gmap.ScriptableReferences.FloatReference EnergyValue;

        public IBrain<InputState> Brain { get; set; }
        public float LastNote { get; private set; }

        float _lastPress = 0f;
        bool _shouldFire;
        bool _canFire = true;
        bool _isSpecial = false;
        float _waitTime = 0.3f;

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

        OnNoteArgs lastNoteArgs;
        protected ShotData lastShotData = new ShotData{};
        protected InputState LastInputState = new InputState{};

        TurntableBehaviour turntable;
        GunBehaviour gun;

        protected virtual void Awake()
        {
            gun = GetComponentInChildren<GunBehaviour>();

            turntable = GetTurntable(false, "");
            turntable.UnityEvent.AddListener(Callback_OnNote);
        }

        protected TurntableBehaviour GetTurntable(bool resolveWithTag, string tag)
        {
            return new TurntableResolver().Get(gameObject);
        }

        public void Callback_OnNote(OnNoteArgs n)
        {
            if (!_canFire)
                return;

            lastNoteArgs = n;
            bool special = Mathf.Abs(Time.time - _lastPress) < _waitTime;
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
            if (LastInputState.Shoot)
                _lastPress = Time.time;
            Energy = Mathf.Clamp01(Energy + Time.deltaTime*0.1f);
        }

        Coroutine waitingForPress;
        IEnumerator WaitForPress()
        {
            if (Brain == null)
                yield break;

            float timeWaited = 0f;
            while (timeWaited < _waitTime)
            {
                timeWaited += Time.deltaTime;
                if (LastInputState.Shoot)
                {
                    FireGun(lastNoteArgs, true);
                    yield break;
                }
                yield return null;
            }
        }

        IEnumerator DisableGun(float time)
        {
            _canFire = false;
            yield return new WaitForSeconds(time);
            _canFire = true;
        }

        protected virtual void FireGun(OnNoteArgs args, bool special)
        {
            _isSpecial = special;
            lastShotData = gun.Fire(new FireRequest
            {
                BulletScale = Mathf.Max(0.01f, args.Duration*5f),
                Special = special
            });

            foreach (var instance in lastShotData.BulletInstances)
            {
                Bullet bullet = instance.GetComponent<Bullet>();
                if (bullet != null)
                    bullet.IsSpecial = special;
            }

            waitingForPress = null;
        }
    }
}