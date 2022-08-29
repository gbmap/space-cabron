using UnityEngine;
using ObjectPool;
using Gmap.Gameplay;

public class DestroyFunction : MonoBehaviour, IObjectPoolEventHandler
{
    public CircleCollider2D circleCollider;
    public bool DamageOnDestroy = false;

    Collider2D[] collisionResults = new Collider2D[10];

    public void AnimDestroy()
    {
        if (DamageOnDestroy)
        {

            ContactFilter2D contactFilter = new ContactFilter2D
            {
                layerMask = LayerMask.GetMask("Enemy")
            };

            Physics2D.OverlapCircle(
                transform.position, 
                circleCollider.radius, 
                contactFilter,
                collisionResults
            );

            foreach (Collider2D collider in collisionResults)
            {
                if (collider != null)
                    collider.GetComponentInChildren<Health>()?.TakeDamage(null, circleCollider);
            }
        }

        this.DestroyOrDisable();
    }

    public void PoolReset()
    {

    }
}
