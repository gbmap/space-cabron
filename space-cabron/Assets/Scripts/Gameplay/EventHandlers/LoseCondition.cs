using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCabron.Gameplay
{
    public class LoseCondition : MonoBehaviour
    {
        public GameState LoseState;
        public GameState GameOverMenuState;

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
            MessageRouter.AddHandler<MsgGameOver>(Callback_GameOver);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
            MessageRouter.RemoveHandler<MsgGameOver>(Callback_GameOver);
        }

        private void Callback_OnObjectDestroyed(MsgOnObjectDestroyed obj)
        {
            if (!obj.health.CompareTag("PlayerChip"))
                return;

            if (GameObject.FindGameObjectsWithTag("Drone").Length > 0)
                return;
            
            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                return;

            MessageRouter.RaiseMessage(new MsgGameOver());
            StartCoroutine(LoseCoroutine());
        }

        IEnumerator LoseCoroutine()
        {
            yield return new WaitForSeconds(1f);
            LoseState.ChangeTo();
            yield return new WaitForSeconds(5f);
            GameOverMenuState.ChangeTo();
        }

        private void Callback_GameOver(MsgGameOver obj)
        {
            StartCoroutine(LoseCoroutine());
        }

    }
}