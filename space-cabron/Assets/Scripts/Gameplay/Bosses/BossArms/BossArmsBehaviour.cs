using System.Collections;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossArmsBehaviour : BossBehaviour
    {
        public Health LeftArmHealth;
        public Health RightArmHealth;
        public Health MainHealth;

        public BossArmsFireHand HandLeft;
        public BossArmsFireHand HandRight;

        public GameObject Bullet;
        public GameObject BulletSinSpeed;
        public Transform BulletSpawnLeft;
        public Transform BulletSpawnRight;

        int MaxHealth
        {
            get
            {
                int total = 0;
                if (LeftArmHealth != null)
                    total += LeftArmHealth.MaxHealth;
                if (RightArmHealth != null)
                    total += RightArmHealth.MaxHealth;
                total += MainHealth.MaxHealth;
                return total;
            }
        }

        int CurrentHealth
        {
            get
            {
                int total = 0;
                if (LeftArmHealth != null)
                    total += LeftArmHealth.CurrentHealth;
                if (RightArmHealth != null)
                    total += RightArmHealth.CurrentHealth;
                total += MainHealth.CurrentHealth;
                return total;
            }
        }

        float BurstShotCooldown => LerpByHealth(1f, 3.00f);

        void Start()
        {
            StartCoroutine(Logic());
            StartCoroutine(ConstantBurstShot(BulletSpawnLeft, BulletSpawnRight));
        }

        IEnumerator Logic()
        {
            while (true)
            {
                yield return HandsLoop(6f, Random.Range(0, 3));
            }
        }

        IEnumerator HandsLoop(float waitTime, int patternIndex)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            yield return FireHands(player);
            switch (patternIndex)
            {
                case 0:
                    yield return ShotPattern1(BulletSpawnLeft, BulletSpawnRight);
                    break;
                case 1:
                    yield return ShotPattern2(BulletSpawnLeft, BulletSpawnRight, player);
                    break;
                case 2:
                    yield return ShotPattern3(BulletSpawnLeft, BulletSpawnRight, player);
                    break;
                default:
                    break;
            }
            yield return RecoverHands();
        }

        IEnumerator FireHands(GameObject player)
        {
            if (HandLeft != null)
                yield return HandLeft.FireHand(player);

            if (HandRight != null)
                yield return HandRight.FireHand(player);
        }

        IEnumerator RecoverHands()
        {
            if (HandRight != null)
                yield return HandRight.RecoverHand();
            if (HandLeft != null)
                yield return HandLeft.RecoverHand();
        }

        public IEnumerator BurstShot(Transform gunTransform, GameObject player)
        {
            float angleToPlayer = GetAngleToPlayer(gunTransform, player);

            for (int i = 0; i < 5; i++)
            {
                Shoot(angleToPlayer + Random.Range(-10f, 10f), gunTransform);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private static float GetAngleToPlayer(Transform gunTransform, GameObject player)
        {
            float angleToPlayer = 180f;
            if (player != null)
                angleToPlayer = Vector3.Angle(
                gunTransform.position,
                player.transform.position
            );
            return angleToPlayer;
        }

        public IEnumerator ConstantBurstShot(Transform gunTransformA, Transform gunTransformB)
        {
            while (true)
            {
                yield return BurstShot(
                    Random.value > 0.5 ? gunTransformA : gunTransformB, 
                    GameObject.FindGameObjectWithTag("Player")
                );
                yield return new WaitForSeconds(BurstShotCooldown);
            }
        }

        IEnumerator ShotPattern1(Transform gunTransformA, Transform gunTransformB)
        {
            int numberOfBursts = LerpByHealth(8, 5);
            int numberOfBullets = LerpByHealth(25, 10);
            float timeBetweenBursts = LerpByHealth(0.25f, 1f);
            for (int j = 0; j < numberOfBursts; j++)
            {
                for (int i = 0; i < numberOfBullets; i++)
                {
                    float angle = 0.5f*j + ((float)i)*360f/numberOfBullets;
                    Shoot(angle, gunTransformA);
                    Shoot(angle, gunTransformB);
                }
                yield return new WaitForSeconds(timeBetweenBursts);
            }
            yield break;
        }

        IEnumerator ShotPattern2(Transform gunTransformA, Transform gunTransformB, GameObject player)
        {
            int numberOfBursts = LerpByHealth(8, 5);
            int numberOfBullets = LerpByHealth(40, 20);
            float timeBetweenBursts = LerpByHealth(0.25f, 1f);
            for (int j = 0; j < numberOfBursts; j++)
            {
                Transform targetTransform = j % 2 == 0 ? gunTransformA : gunTransformB;
                float angleToPlayer = GetAngleToPlayer(targetTransform, player);

                for (int i = 0; i < numberOfBullets; i++)
                {
                    float angle = angleToPlayer + Mathf.Sin((Random.value+0.1f)*((float)i)*Mathf.PI*2f)*25f;
                    Shoot(angle, targetTransform, 1);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(timeBetweenBursts);
            }
            yield break;
        }

        IEnumerator ShotPattern3(Transform gunTransformA, Transform gunTransformB, GameObject player)
        {
            int numberOfBursts = LerpByHealth(8, 5);
            int numberOfBullets = LerpByHealth(25, 10);
            float timeBetweenBursts = LerpByHealth(0.25f, 1f);
            for (int j = 0; j < numberOfBursts; j++)
            {
                float initialAngle = Random.Range(0f, 360f);
                for (int i = 0; i < numberOfBullets; i++)
                {
                    float angle = initialAngle*j + ((float)i)*360f/numberOfBullets;
                    Shoot(angle, gunTransformA, 1);
                    Shoot(angle, gunTransformB, 1);
                    yield return new WaitForSeconds(0.05f);
                }
                yield return new WaitForSeconds(timeBetweenBursts);
            }

            yield return ShotPattern1(gunTransformA, gunTransformB);
        }

        private GameObject Shoot(float angle, Transform gunTransform, int bulletType=0)
        {
            var instance = Instantiate(
                bulletType == 0 ? Bullet : BulletSinSpeed, 
                gunTransform.position,
                Quaternion.Euler(0f, 0f, angle)
            );
            return instance;
        }
    }
}
