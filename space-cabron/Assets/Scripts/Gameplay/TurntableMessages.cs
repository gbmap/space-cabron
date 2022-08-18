using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Gameplay
{    
    public class MsgOnImprovisationAdded
    {
        public GameObject Object;
        public ITurntable Turntable;
        public Improvisation Improvisation;
    }

    public class MsgOnImprovisationRemoved : MsgOnImprovisationAdded {}

    public class TurntableMessages : MonoBehaviour
    {
        TurntableBehaviour turntable;

        void Awake()
        {
            turntable = GetComponent<TurntableBehaviour>();
            turntable.OnImprovisationAdded += Callback_OnImprovisationAdded;
            turntable.OnImprovisationRemoved += Callback_OnImprovisationRemoved;
        }

        void OnDestroy()
        {
            if (!turntable)
                return;

            turntable.OnImprovisationAdded -= Callback_OnImprovisationAdded;
            turntable.OnImprovisationRemoved -= Callback_OnImprovisationRemoved;
        }

        private void Callback_OnImprovisationAdded(OnImprovisationArgs msg)
        {
            MessageRouter.RaiseMessage(new MsgOnImprovisationAdded {
                Object = gameObject,
                Turntable = msg.Turntable,
                Improvisation = msg.Improvisation
            });
        }
        
        private void Callback_OnImprovisationRemoved(OnImprovisationArgs msg)
        {
            MessageRouter.RaiseMessage(new MsgOnImprovisationRemoved {
                Object = gameObject,
                Turntable = msg.Turntable,
                Improvisation = msg.Improvisation
            });
        }
    }
}