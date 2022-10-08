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

        private int StartLevelScore;

        void OnEnable()
        {
            MessageRouter.AddHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
            MessageRouter.AddHandler<Messages.MsgOnComboBroken>(Callback_OnComboBroken);
            MessageRouter.AddHandler<Messages.MsgLevelFinishedLoading>(Callback_LevelFinishedLoading);
            MessageRouter.AddHandler<Messages.MsgOnRetry>(Callback_OnRetry);
            MessageRouter.AddHandler<Messages.MsgLevelWon>(Callback_LevelWon);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
            MessageRouter.RemoveHandler<Messages.MsgOnComboBroken>(Callback_OnComboBroken);
            MessageRouter.RemoveHandler<Messages.MsgLevelFinishedLoading>(Callback_LevelFinishedLoading);
            MessageRouter.RemoveHandler<Messages.MsgLevelWon>(Callback_LevelWon);
            MessageRouter.RemoveHandler<Messages.MsgOnRetry>(Callback_OnRetry);
        }

        private void Callback_LevelWon(MsgLevelWon msg)
        {
            StartLevelScore = TotalScore.Value;
        }

        private void Callback_OnRetry(MsgOnRetry msg)
        {
            TotalScore.Value = StartLevelScore;
            MessageRouter.RaiseMessage(
                new Messages.MsgOnScoreChanged(0, TotalScore.Value)
            );
        }

        private void Callback_LevelFinishedLoading(MsgLevelFinishedLoading obj)
        {
            Debug.Log("Resetting score...");
            CurrentScore = 0;
            int totalScore = TotalScore.Value;
            TotalScore.Value = StartLevelScore;
            StartLevelScore = totalScore;
        }

        void Start()
        {
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(0, TotalScore.Value));
        }

        private void Callback_OnComboBroken(MsgOnComboBroken msg)
        {
            Callback_IncreaseScore(new MsgIncreaseScore(msg.Combo*2));         
        }

        private void Callback_IncreaseScore(Messages.MsgIncreaseScore msg)
        {
            CurrentScore += msg.Value;
            TotalScore.Value += msg.Value;
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(CurrentScore, TotalScore.Value));
        }
    }
}