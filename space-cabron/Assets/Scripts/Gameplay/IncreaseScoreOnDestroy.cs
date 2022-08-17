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

        private void Callback_OnDestroy(MsgOnObjectDestroyed obj)
        {
            if (obj.bullet == null)
                return;
            
            if (obj.health.CompareTag("Player"))
                return;
            
            if (obj.health.CompareTag("Drone"))
                return;

            MessageRouter.RaiseMessage(
                new SpaceCabron.Messages.MsgIncreaseScore(Value)
            );
        }
    }
}