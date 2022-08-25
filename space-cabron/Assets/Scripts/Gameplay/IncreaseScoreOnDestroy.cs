using Frictionless;
using SpaceCabron.Gameplay;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class IncreaseScoreOnDestroy : MonoBehaviour
    {
        public int Value;
        Health health;

        public GameObject ScoreLabel;
        
        void Awake()
        {
            health = GetComponent<Health>();
            health.OnDestroy += Callback_OnDestroy;
        }

        private void Callback_OnDestroy(MsgOnObjectDestroyed obj)
        {
            if (obj.bullet == null && obj.collider == null)
                return;
            
            if (obj.health.CompareTag("Player"))
                return;
            
            if (obj.health.CompareTag("Drone"))
                return;

            var scoreInstance = Instantiate(
                ScoreLabel, 
                transform.position, 
                Quaternion.identity
            );
            scoreInstance.GetComponent<ScoreLabel>().Score = Value;

            MessageRouter.RaiseMessage(
                new SpaceCabron.Messages.MsgIncreaseScore(Value)
            );
        }
    }
}