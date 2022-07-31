using Frictionless;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseScoreMessage
{
    public uint Value;
}

public class ScoreController : MonoBehaviour
{
    public Text ScoreText;

    uint _score;

    private void OnEnable()
    {
        MessageRouter.AddHandler<IncreaseScoreMessage>(OnIncreaseScore);
    }

    private void OnDisable()
    {
        MessageRouter.RemoveHandler<IncreaseScoreMessage>(OnIncreaseScore);
    }

    private void OnIncreaseScore(IncreaseScoreMessage msg)
    {
        IncreaseScore(msg.Value);
    }

    public void IncreaseScore(uint value)
    {
        _score += value;
        ScoreText.text = _score.ToString();
    }
}
