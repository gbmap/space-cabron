using Frictionless;
using UnityEngine;

namespace Gmap.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Gameplay/State")]
    public class GameState : ScriptableObject 
    {
        public static GameState Current { get; private set; }
        public void ChangeTo()
        {
            MessageRouter.RaiseMessage(new MsgOnStateChanged
            {
                PreviousState = Current,
                NewState = this
            });
            Current = this;
        }
    }

    public class MsgRequestChangeState 
    {
        public GameState State;
    }

    public class MsgOnStateChanged
    {
        public GameState PreviousState;
        public GameState NewState;
    }

    public interface IGameStateListener
    {
        void OnStateChanged(MsgOnStateChanged msg);
    }
}