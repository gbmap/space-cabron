using UnityEngine;

namespace Gmap.Gun
{
    [System.Serializable]
    public class BulletData
    {
        public GameObject Prefab;
    }

    public class GunBehaviour : MonoBehaviour
    {
        public BulletData Bullet;
        public GunData GunData;

        private ShotData m_lastShot;

        public void Fire()
        {
            ShotRequest shotRequest = new ShotRequest
            {
                Position = transform.position,
                Gun = GunData,
                Bullet = this.Bullet
            };

            m_lastShot = GunData.Fire(m_lastShot, shotRequest);
        }
    }
}