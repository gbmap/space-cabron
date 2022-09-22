using Gmap.Gun;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class Bullet : MonoBehaviour
    {
        public bool IsSpecial;
        public bool DestroyOnCollision = true;
        public ShotData ShotData;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var health = collider.GetComponentInParent<Health>();
            if (health)
            {
                health.TakeDamage(this, collider, ShotData?.ObjectFiring);
            }
            
            if (DestroyOnCollision) {
                TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
                if (tr != null) {
                    tr.time = tr.time / 2f;
                    tr.transform.parent = null;
                }
                this.DestroyOrDisable();
            }
        }
    }
}