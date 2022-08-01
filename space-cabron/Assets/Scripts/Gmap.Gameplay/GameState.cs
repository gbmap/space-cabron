using Frictionless;
using UnityEngine;

namespace Gmap.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Gameplay/State")]
    public class GameState : ScriptableObject 
    {
        public void ChangeTo()
        {
            MessageRouter.RaiseMessage(new MsgOnStateChanged
            {
                NewState = this
            });
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