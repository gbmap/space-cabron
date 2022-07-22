using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Scoreboard.Messages;
using UnityEngine;

namespace SpaceCabron.Scoreboard.UI
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class ScoreUIText : MonoBehaviour
    {
        TMPro.TextMeshProUGUI text;
        void Awake()
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }

        void OnEnable()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().AddHandler<Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        void OnDisable()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RemoveHandler<Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(MsgOnScoreChanged msg)
        {
            text.text = msg.Score.ToString("000000000");
        }
    }
}