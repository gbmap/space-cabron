using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        var health = collider.GetComponent<Health>();
        if (health)
        {
            health.TakeDamage(this, collider);
        }
        
        this.DestroyOrDisable();
    }
}
