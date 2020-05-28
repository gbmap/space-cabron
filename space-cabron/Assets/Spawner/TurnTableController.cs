using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTableController : MonoBehaviour
{
    public TurnTable turntable;

    public Text txtMaxSubBeats;
    public Text txtNBeats;

    private void SetMaxSubBeats(int value)
    {
        txtMaxSubBeats.text = value.ToString();
        LoopCreator.MaxSubBeats = value;
    }

    public void IncreaseMaxSubBeats()
    {
        SetMaxSubBeats(LoopCreator.MaxSubBeats + 1);
    }

    public void DecreaseMaxSubBeats()
    {
        SetMaxSubBeats(LoopCreator.MaxSubBeats + 1);
    }

    private void SetNBeats(int value)
    {
        txtMaxSubBeats.text = value.ToString();
        turntable.NBeats = value;
    }

    public void IncreaseNBeats()
    {
        SetNBeats(turntable.NBeats + 1);
    }

    public void DecreaseNBeats()
    {
        SetNBeats(turntable.NBeats - 1);
    }


}
