using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class PauseGame : MonoBehaviour
    {
        public GameState PauseState;
        public GameState GameplayState;

        MessageRouter router;
        MessageRouter Router
        {
            get
            {
                return router ?? (router = ServiceFactory.Instance.Resolve<MessageRouter>());
            }
        }

        void OnEnable()
        {
            Router.AddHandler<MsgPauseGame>(Callback_PauseGameCallback);
        }

        void OnDisable()
        {
            Router.RemoveHandler<MsgPauseGame>(Callback_PauseGameCallback);
        }

        void Callback_PauseGameCallback(MsgPauseGame msg)
        {
            Time.timeScale = msg.Value ? 0f : 1f;
            GameState state = msg.Value ? PauseState : GameplayState;
            state.ChangeTo();
        }
    }
}