using UnityEngine;

public class Message { }

public class MsgOnEnemyHit : Message
{
    public Bullet bullet;
    public Collider2D collider;
    public Health enemy;
}

public class MsgOnEnemyDestroyed : MsgOnEnemyHit
{ }

public class MsgOnWaveEnded : Message
{
    public Wave Wave;
}
