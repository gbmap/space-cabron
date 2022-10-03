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
            MessageRouter.AddHandler<Messages.MsgOnComboBroken>(Callback_OnComboBroken);
            MessageRouter.AddHandler<Messages.MsgLevelFinishedLoading>(Callback_LevelFinishedLoading);
        }

        private void Callback_LevelFinishedLoading(MsgLevelFinishedLoading obj)
        {
            Debug.Log("Resetting score...");
            CurrentScore = 0;
        }

        void Start()
        {
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(0, TotalScore.Value));
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
            MessageRouter.RemoveHandler<Messages.MsgOnComboBroken>(Callback_OnComboBroken);
            MessageRouter.RemoveHandler<Messages.MsgLevelFinishedLoading>(Callback_LevelFinishedLoading);
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