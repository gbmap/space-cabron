using UnityEngine;

namespace SpaceCabron.Messages {
    public class RandomizeBeat {}
}
public class Message { }

public class MsgOnEnemyHit : Message
{
    public string enemyName;
    public Bullet bullet;
    public Collider2D collider;
    public Health enemy;
}

public class MsgOnEnemyDestroyed : MsgOnEnemyHit
{ }


