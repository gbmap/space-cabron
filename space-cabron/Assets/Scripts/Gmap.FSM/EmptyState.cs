using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.FSM
{
    public class EmptyState : FSMState
    {
        public float WaitTime = 1f;
        public override void OnEnterState(FSMState previousState){}
        public override void OnExitState(FSMState nextState){}

        public void WaitAndChangeState(FSMState nextState)
        {
            StartCoroutine(WaitAndChangeStateCoroutine(nextState));
        }

        private IEnumerator WaitAndChangeStateCoroutine(FSMState nextState)
        {
            yield return new WaitForSeconds(WaitTime);
            GetComponentInParent<StateMachineBehaviour>().ChangeState(nextState);
        }
    }
}