using System;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Scoreboard
{
    public class Score : MonoBehaviour
    {
        int CurrentScore;

        void OnEnable()
        {
            MessageRouter.AddHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
        }

        private void Callback_IncreaseScore(Messages.MsgIncreaseScore msg)
        {
            CurrentScore += msg.Value;
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(CurrentScore));
        }
    }
}