using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace SpaceCabron.Scoreboard
{
    public class IncreaseScoreOnDestroy : MonoBehaviour
    {
        public int Value;

        void OnDestroy()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RaiseMessage(
                new Messages.MsgIncreaseScore(Value)
            );
        }
    }
}