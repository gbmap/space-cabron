using Frictionless;
using UnityEngine;
using Gmap.ScriptableReferences;

namespace SpaceCabron.Scoreboard
{
    public class Score : MonoBehaviour
    {
        public IntReference TotalScore;
        int CurrentScore;

        void OnEnable()
        {
            MessageRouter.AddHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
        }

        void Start()
        {
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(0, TotalScore.Value));
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgIncreaseScore>(Callback_IncreaseScore);
        }

        private void Callback_IncreaseScore(Messages.MsgIncreaseScore msg)
        {
            CurrentScore += msg.Value;
            TotalScore.Value += msg.Value;
            MessageRouter.RaiseMessage(new Messages.MsgOnScoreChanged(CurrentScore, TotalScore.Value));
        }
    }
}