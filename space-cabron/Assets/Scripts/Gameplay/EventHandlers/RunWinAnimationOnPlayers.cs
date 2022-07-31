using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;
using System.Threading;
using Gmap.Gameplay;

namespace SpaceCabron.Gameplay
{
    public class RunWinAnimationOnPlayers : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        int winAnimationCount = 0;
        LevelConfiguration levelConfiguration;

        void OnEnable()
        {
            MessageRouter.AddHandler<SpaceCabron.Messages.MsgLevelWon>(Callback_LevelWon);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<SpaceCabron.Messages.MsgLevelWon>(Callback_LevelWon);
        }

        private void Callback_LevelWon(MsgLevelWon msg)
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
            winAnimationCount = go.Length;
            foreach (GameObject player in go)
                VictoryBrain.Play(player, OnVictoryAnimationEnded);
            StartCoroutine(WaitVictoryAnimationEnded());
        }

        private void OnVictoryAnimationEnded()
        {
            winAnimationCount--;
        }

        private IEnumerator WaitVictoryAnimationEnded()
        {
            while (winAnimationCount > 0)
                yield return new WaitForSecondsRealtime(0.1f);

            if (levelConfiguration != null && levelConfiguration.NextLevel != null)
                LevelLoader.Load(levelConfiguration.NextLevel);
            else
                throw new System.Exception("No next level.");
        }

        public void Configure(LevelConfiguration configuration)
        {
            levelConfiguration = configuration;
        }
    }
}