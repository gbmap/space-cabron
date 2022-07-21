using UnityEngine;
using Frictionless;
using SpaceCabron.Scoreboard.Messages;

namespace SpaceCabron.Gameplay
{

    public class TimelineEvent
    {
        int Score;
    }

    public class ScoreTimeline : MonoBehaviour
    {
        void Awake()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().AddHandler<Scoreboard.Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        void OnDestroy()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RemoveHandler<Scoreboard.Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(MsgOnScoreChanged msg)
        {

        }
    }
}