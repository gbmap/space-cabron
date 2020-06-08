using Frictionless;
using ObjectPool;
using UnityEngine;

public enum EUpgrade
{
    // beat maker
    BPM,
    MaxSubBeats,
    BeatsInBar,
    NBeats,

    AddNoteToPlayerSynth,
    AddNoteToEnemyDrum
}

public class MsgOnUpgradeTaken
{
    public EUpgrade Type;
    public object Value;
}

public class Upgrade : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
{
    [HideInInspector]
    public EUpgrade Type;

    [HideInInspector]
    public object Value;

    MessageRouter _msg;

    private void Awake()
    {
        Type = EUpgrade.BPM;
        Value = 10;

        _msg = ServiceFactory.Instance.Resolve<MessageRouter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _msg.RaiseMessage(new MsgOnUpgradeTaken
        {
            Type = Type,
            Value = Value
        });

        this.DestroyOrDisable();
    }

    void IObjectPoolEventHandler.PoolReset()
    {

    }
}
