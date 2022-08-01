using Frictionless;
using ObjectPool;
using UnityEngine;

public enum EUpgrade
{
    // beat maker
    BPM,
    GeneratePlayerMelody,
    GeneratePlayerBeat,
    GenerateEnemyMelody,
    GenerateEnemyBeat,
    AddMelodyImproviserToPlayer,
    AddMarchImproviserToPlayer,
    AddMarchImproviserToEnemy
}

public class MsgOnUpgradeTaken
{
    public EUpgrade Type;
    public object Value;
}

public class Upgrade : MonoBehaviour, ObjectPool.IObjectPoolEventHandler
{
    public UpgradeConfiguration config;
    public new SpriteRenderer renderer;

    public EUpgrade Type;
    public object Value;

    void OnEnable()
    {
        renderer.sprite = config.GetSprite(Type);
    }

    public void SetType(EUpgrade type, object value = null)
    {
        Type = type;
        Value = value;
        renderer.sprite = config.GetSprite(Type);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MessageRouter.RaiseMessage(new MsgOnUpgradeTaken
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
