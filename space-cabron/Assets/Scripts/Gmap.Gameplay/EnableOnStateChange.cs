using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class EnableOnStateChange : MonoBehaviour
    {
        public GameState State;
        public GameObject TargetObject;
        public AudioClip SoundEffect;

        void Awake()
        {
            MessageRouter.AddHandler<MsgOnStateChanged>(OnStateChanged);
        }

        void OnDestroy()
        {
            MessageRouter.RemoveHandler<MsgOnStateChanged>(OnStateChanged);
        }

        public void OnStateChanged(MsgOnStateChanged msg)
        {
            if (msg.NewState == State) {
                if (SoundEffect != null) {
                    AudioSource.PlayClipAtPoint(SoundEffect, Camera.main.transform.position);
                }
            }
            TargetObject.SetActive(msg.NewState == State);
        }
    }
}