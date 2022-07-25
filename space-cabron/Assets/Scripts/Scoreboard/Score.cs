using System;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Scoreboard
{
    public class Score : MonoBehaviour
    {
        int CurrentScore;
        MessageRouter router;

        void Awake()
        {
            router = ServiceFactory.Instance.Resolve<MessageRouter>(); 
        }

        void OnEnable()
        {
            router.AddHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
        }

        void OnDisable()
        {
            router.RemoveHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
        }

        private void Callback_IncreaseScore(Messages.MsgIncreaseScore msg)
        {
            CurrentScore += msg.Value;
            router.RaiseMessage(new Messages.MsgOnScoreChanged(CurrentScore));
        }
    }
}