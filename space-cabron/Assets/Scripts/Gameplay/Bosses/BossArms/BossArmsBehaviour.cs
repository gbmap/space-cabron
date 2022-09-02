using System.Collections;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class BossArmsBehaviour : MonoBehaviour
    {
        public BossArmsFireHand HandLeft;
        public BossArmsFireHand HandRight;

        public GameObject Bullet;
        public Transform BulletSpawnLeft;
        public Transform BulletSpawnRight;

        void Start()
        {
            StartCoroutine(Stage1());
        }

        IEnumerator Stage1()
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (HandLeft != null)
                yield return HandLeft.FireHand(player);

            if (HandRight != null)
                yield return HandRight.FireHand(player);

            for (int i = 0; i < 3; i++)
            {
                yield return BurstShot(
                    Random.value < 0.5 ? BulletSpawnLeft : BulletSpawnRight, 
                    player
                );
                yield return new WaitForSeconds(2f);
            }

            if (HandRight != null)
                yield return HandRight.RecoverHand();
            if (HandLeft != null)
                yield return HandLeft.RecoverHand();
        }

        public IEnumerator BurstShot(Transform gunTransform, GameObject player)
        {
            float angleToPlayer = 180f;
            if (player != null)
                angleToPlayer = Vector3.Angle(
                gunTransform.position, 
                player.transform.position
            );

            for (int i = 0; i < 5; i++)
            {
                var instance = Instantiate(
                    Bullet, 
                    BulletSpawnLeft.position,
                    Quaternion.Euler(0f, 0f, angleToPlayer + Random.Range(-10f, 10f))
                );
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
