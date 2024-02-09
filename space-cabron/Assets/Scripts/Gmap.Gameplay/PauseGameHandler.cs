using System;
using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.Events;

public class PauseGameHandler : MonoBehaviour
{
    public UnityEvent OnPause;
    public UnityEvent OnResume;

    void OnEnable()
    {
        MessageRouter.AddHandler<SpaceCabron.Messages.MsgPauseGame>(Callback_PauseGameCallback);
    }

    private void Callback_PauseGameCallback(MsgPauseGame msg)
    {
        UnityEvent e = msg.Value ? OnPause : OnResume;
        e.Invoke();
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<SpaceCabron.Messages.MsgPauseGame>(Callback_PauseGameCallback);
    }
}