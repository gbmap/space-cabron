using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class EnableOnStateChange : MonoBehaviour, IGameStateListener
    {
        public GameState State;
        public GameObject TargetObject;

        void OnEnable()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().AddHandler<MsgOnStateChanged>(OnStateChanged);
        }

        void OnDisable()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RemoveHandler<MsgOnStateChanged>(OnStateChanged);
        }

        public void OnStateChanged(MsgOnStateChanged msg)
        {
            TargetObject.SetActive(msg.NewState == State);
        }
    }
}