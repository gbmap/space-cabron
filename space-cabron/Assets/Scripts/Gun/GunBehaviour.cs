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
        public MonoBehaviour ObjectFiring;

        public FireRequest(float bulletScale, bool special, MonoBehaviour objectFiring)
        {
            BulletScale = bulletScale;
            Special = special;
            ObjectFiring = objectFiring;
        }
    }

    public class GunBehaviour : MonoBehaviour
    {
        public BulletData Bullet;
        public GunData GunData;

        public ShotData LastShot { get; private set; }

        public ShotData Fire(FireRequest request)
        {
            ShotRequest shotRequest = new ShotRequest
            {
                Position = transform.position,
                Gun = GunData,
                Bullet = this.Bullet,
                Special = request.Special,
                BulletScale = request.BulletScale,
                ObjectFiring = request.ObjectFiring
            };
            LastShot = GunData.Fire(LastShot, shotRequest);
            return LastShot;
        }

        public void Fire()
        {
            Fire(new FireRequest(1f, false, null));
        }
    }
}