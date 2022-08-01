using UnityEngine;

namespace Gmap.Gun
{
    [System.Serializable]
    public class BulletData
    {
        public GameObject Prefab;
    }

    public class FireRequest
    {
        public float BulletScale;
        public bool Special;
    }

    public class GunBehaviour : MonoBehaviour
    {
        public BulletData Bullet;
        public GunData GunData;

        private ShotData lastShot;

        public ShotData Fire(FireRequest request)
        {
            ShotRequest shotRequest = new ShotRequest
            {
                Position = transform.position,
                Gun = GunData,
                Bullet = this.Bullet,
                Special = request.Special,
                BulletScale = request.BulletScale
                
            };
            lastShot = GunData.Fire(lastShot, shotRequest);
            return lastShot;
        }
    }
}