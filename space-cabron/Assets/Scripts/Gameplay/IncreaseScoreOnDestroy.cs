using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class IncreaseScoreOnDestroy : MonoBehaviour
    {
        public int Value;
        Health health;
        
        void Awake()
        {
            health = GetComponent<Health>();
            health.OnDestroy += Callback_OnDestroy;
        }

        private void Callback_OnDestroy(MessageOnEnemyDestroyed obj)
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RaiseMessage(
                new Gmap.Messages.MsgIncreaseScore(Value)
            );
        }
    }
}