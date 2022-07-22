using UnityEngine;

namespace Gmap.Gun
{    

    public class ShotRequest
    {
        public Vector3 Position;
        public GunData Gun;
        public BulletData Bullet;
        public bool Special;
    }

    public class ShotData
    {
        public float Time;
        public Vector3 Position;
        public GameObject[] BulletInstances;
    }
    
    /// <summary>
    /// Describes the behaviour of a gun. Bullets per shot, accuracy,
    /// recover time, etc.
    /// </summary>
    [CreateAssetMenu(menuName="Gmap/Gun/Gun Data")]
    public class GunData : ScriptableObject
    {
        public float ShotsPerSecond = 2;

        public bool CanFire(ShotData lastShot)
        {
            return (Time.time - lastShot.Time) > (1f / ShotsPerSecond);

        }

        public ShotData Fire(ShotData lastShot, ShotRequest shotRequest)
        {
            if (lastShot != null && !CanFire(lastShot))
                return lastShot;

            if (shotRequest.Special)
            {
                Vector3 shotPosition = GetBulletPosition(lastShot, shotRequest.Position);
                Quaternion shotRotation = GetBulletRotation(lastShot);
                GameObject instance = Instantiate(
                    shotRequest.Bullet.Prefab, 
                    shotPosition + Vector3.left * 0.15f, 
                    shotRotation
                );

                GameObject instance2 = Instantiate(
                    shotRequest.Bullet.Prefab, 
                    shotPosition + Vector3.right * 0.15f, 
                    shotRotation
                );

                return new ShotData
                {
                    BulletInstances = new GameObject[]{instance, instance2},
                    Position = shotPosition,
                    Time = Time.time,
                };
            }
            else
            {
                Vector3 shotPosition = GetBulletPosition(lastShot, shotRequest.Position);
                Quaternion shotRotation = GetBulletRotation(lastShot);

                GameObject instance = Instantiate(
                    shotRequest.Bullet.Prefab, 
                    shotPosition, 
                    shotRotation
                );

                return new ShotData
                {
                    BulletInstances = new GameObject[]{instance},
                    Position = shotPosition,
                    Time = Time.time,
                };
            }
        }

        public Vector3 GetBulletPosition(ShotData lastShot, Vector3 position)
        {
            return position;
        }

        public Quaternion GetBulletRotation(ShotData lastShot)
        {
            return Quaternion.identity;
        }
    }
}