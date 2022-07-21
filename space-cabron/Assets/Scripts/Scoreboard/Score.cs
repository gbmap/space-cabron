using System;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Scoreboard
{
    namespace Messages 
    {
        public class MsgOnScoreChanged
        {
            public int Score;
            public MsgOnScoreChanged(int score)
            {
                Score = score;
            }
        }

        public class MsgIncreaseScore
        {
            public int Value;
            public MsgIncreaseScore(int value)
            {
                Value = value;
            }
        }
    }

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