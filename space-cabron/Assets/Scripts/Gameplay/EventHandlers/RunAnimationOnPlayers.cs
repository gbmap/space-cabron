using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;
using System.Threading;
using Gmap.Gameplay;
using UnityEngine.SceneManagement;

namespace SpaceCabron.Gameplay
{
    public class RunAnimationOnPlayers : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        int animationCount = 0;
        LevelConfiguration levelConfiguration;

        void OnEnable()
        {
            MessageRouter.AddHandler<SpaceCabron.Messages.MsgLevelWon>(Callback_LevelWon);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<SpaceCabron.Messages.MsgLevelWon>(Callback_LevelWon);
        }

        public void PlayAnimation<T>(System.Action OnAnimationEnded=null) where T : AnimationBrain
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
            animationCount = go.Length;
            foreach (GameObject player in go)
                AnimationBrain.Play<T>(player, ()=>animationCount--);
            StartCoroutine(WaitAnimationEnded(OnAnimationEnded));
        }

        private void Callback_LevelWon(MsgLevelWon msg)
        {
            LevelLoader.Load(levelConfiguration.NextLevel);
            // PlayAnimation<VictoryBrain>(() => { 
            //     if (levelConfiguration != null && levelConfiguration.NextLevel != null)
            //         LevelLoader.Load(levelConfiguration.NextLevel);
            //     else
            //         SceneManager.LoadScene("Gameplay");
            // });
        }

        private IEnumerator WaitAnimationEnded(System.Action OnAnimationEnded)
        {
            while (animationCount > 0)
                yield return null;
            
            OnAnimationEnded?.Invoke();
        }

        public void Configure(LevelConfiguration configuration)
        {
            levelConfiguration = configuration;
        }
    }
}