using UnityEngine;

namespace Gmap.FSM
{
    public abstract class FSMState : MonoBehaviour
    {
        public abstract void OnEnterState(FSMState previousState);
        public UnityEngine.Events.UnityEvent OnEnterStateEvent;

        public abstract void OnExitState(FSMState nextState);
        public UnityEngine.Events.UnityEvent OnExitStateEvent;
    }

    public class StateMachineBehaviour : MonoBehaviour
    {
        public FSMState InitialState;

        void Start()
        {
            ChangeState(InitialState);
        }

        FSMState currentState;
        public void ChangeState(FSMState state) {
            FSMState previousState = currentState;
            if (currentState != null)
            {
                currentState.OnExitStateEvent.Invoke();
                currentState.OnExitState(state);
                currentState.enabled = false;
            }
            currentState = state;
            currentState.enabled = true;
            currentState.OnEnterState(previousState);
            currentState.OnEnterStateEvent.Invoke();
        }
    }
}