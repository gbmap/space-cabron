using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
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
            MessageRouter.AddHandler<Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(Messages.MsgOnScoreChanged msg)
        {
            text.text = msg.TotalScore.ToString("000000000");
        }
    }
}