using Frictionless;
using UnityEngine;
using Gmap.ScriptableReferences;
using SpaceCabron.Messages;
using System;

namespace SpaceCabron.Scoreboard
{
    public class Score : MonoBehaviour
    {
        public IntReference TotalScore;
        public int CurrentScore { get; private set ;}

        void OnEnable()
        {
            MessageRouter.AddHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
            MessageRouter.AddHandler<Messages.MsgLevelFinishedLoading>(Callback_LevelFinishedLoading);
        }

        private void Callback_LevelFinishedLoading(MsgLevelFinishedLoading obj)
        {
            CurrentScore = 0;
        }

        void Start()
        {
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(0, TotalScore.Value));
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
            MessageRouter.RemoveHandler<Messages.MsgLevelFinishedLoading>(Callback_LevelFinishedLoading);
        }

        private void Callback_IncreaseScore(Messages.MsgIncreaseScore msg)
        {
            CurrentScore += msg.Value;
            TotalScore.Value += msg.Value;
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(CurrentScore, TotalScore.Value));
        }
    }
}