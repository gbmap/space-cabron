using UnityEngine;

namespace Gmap.Gameplay
{
    public class Bullet : MonoBehaviour
    {
        public bool IsSpecial;
        public bool DestroyOnCollision = true;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var health = collider.GetComponent<Health>();
            if (health)
            {
                health.TakeDamage(this, collider);
            }
            
            if (DestroyOnCollision)
                this.DestroyOrDisable();
        }
    }
}