using System.Collections;
using System.Collections.Generic;
using Frictionless;
using SpaceCabron.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayMenu : MonoBehaviour
{
    public void Resume() {
        MessageRouter.RaiseMessage(new MsgPauseGame(false));
    }

    public void Exit() {
        Resume();
        SceneManager.LoadScene("Menu");
    }
}
