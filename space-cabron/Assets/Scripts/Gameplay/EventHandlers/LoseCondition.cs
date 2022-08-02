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
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        }

        private void Callback_OnObjectDestroyed(MsgOnObjectDestroyed obj)
        {
            if (!obj.health.CompareTag("Player"))
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
    }
}