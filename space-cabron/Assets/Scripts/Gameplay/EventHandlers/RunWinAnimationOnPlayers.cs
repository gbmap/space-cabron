using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;
using System.Threading;

namespace SpaceCabron.Gameplay
{
    public class RunWinAnimationOnPlayers : MonoBehaviour
    {
        int winAnimationCount = 0;

        void OnEnable()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().AddHandler<SpaceCabron.Messages.MsgLevelWon>(Callback_LevelWon);
        }

        void OnDisable()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RemoveHandler<SpaceCabron.Messages.MsgLevelWon>(Callback_LevelWon);
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
            float timeStart = Time.time;
            while (winAnimationCount > 0)
                yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}