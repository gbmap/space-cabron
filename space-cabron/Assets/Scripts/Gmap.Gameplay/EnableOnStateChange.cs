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
            TargetObject.SetActive(msg.NewState == State);
        }
    }
}