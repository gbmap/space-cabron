using Frictionless;
using UnityEngine;

public enum EUpgrade
{
    BPM
}

public class MsgOnUpgradeTaken
{
    public EUpgrade Type;
    public int Value;
}

public class Upgrade : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
{
    public EUpgrade Type;

    [Range(-10, 10)]
    public int Value;

    MessageRouter _msg;

    public void PoolReset()
    {
        Value = Random.Range(-10, 11);
    }

    private void Awake()
    {
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
}
