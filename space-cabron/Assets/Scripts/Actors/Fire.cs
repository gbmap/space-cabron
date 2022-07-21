using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.CosmicMusicUtensil;
using Gmap.Gun;
using UnityEngine;

namespace SpaceCabron
{
    public class Fire : MonoBehaviour, IBrainHolder
    {
        public IBrain Brain { get; set; }
        float _lastPress = 0f;
        float _lastNote = 0f;
        bool _shouldFire;
        bool _canFire = true;
        bool _isSpecial = false;
        float _waitTime = 0.125f;

        OnNoteArgs _lastNoteArgs;
        TurntableBehaviour turntable;
        GunBehaviour gun;

        void Awake()
        {
            gun = GetComponentInChildren<GunBehaviour>();
            turntable = GetComponentInChildren<TurntableBehaviour>();
            turntable.UnityEvent.AddListener(Callback_OnNote);
        }

        public void Callback_OnNote(OnNoteArgs n)
        {
            if (!_canFire)
                return;

            _lastNoteArgs = n;
            bool special = Mathf.Abs(Time.time - _lastPress) < _waitTime;
            if (special)
            {
                _isSpecial = true;
                gun.Fire(special);
            }
            else 
            {
                waitingForPress = StartCoroutine(WaitForPress());
            }
            _lastNote = Time.time;
        }

        void Update()
        {
            if (Brain.GetInputState().Shoot && waitingForPress == null && !_isSpecial)
            {
                StartCoroutine(DisableGun(1f));
            }
        }

        Coroutine waitingForPress;
        IEnumerator WaitForPress()
        {
            float timeWaited = 0f;
            while (timeWaited < _waitTime)
            {
                timeWaited += Time.deltaTime;
                if (Brain.GetInputState().Shoot)
                {
                    gun.Fire(true);
                    _isSpecial = true;
                    waitingForPress = null;
                    yield break;
                }
                yield return null;
            }
            _isSpecial = false;
            gun.Fire(false);
            waitingForPress = null;
        }

        IEnumerator DisableGun(float time)
        {
            _canFire = false;
            yield return new WaitForSeconds(time);
            _canFire = true;
        }
    }
}