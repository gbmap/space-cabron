using System.Collections;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossCannonNode : BossBehaviour
    {
        public Vector3 Direction;
        public float Speed = 5f;
        [SerializeField] GameObject bullet;
        [SerializeField] GameObject bulletBounce;
        [SerializeField] GameObject bulletPointy;
        [SerializeField] Transform shootTransform;

        float Distance = 2f;

        public bool IsExtended
        {
            get
            {
                return transform.localPosition.sqrMagnitude >= Distance*Distance;
            }
        }

        public bool IsContracted
        {
            get { return transform.localPosition.sqrMagnitude <= 0.1f; }
        }

        public IEnumerator Extend(int shotPattern=-1, float distance=2f)
        {
            StopAllCoroutines();

            Coroutine fireCoroutine = FireBehaviour(shotPattern);
            while (this != null && !IsExtended)
            {
                transform.localPosition += Direction * Speed * Time.deltaTime;
                yield return null;
            }

            if (this == null)
                yield break;

            if (fireCoroutine != null)
                StopCoroutine(fireCoroutine);
        }

        public IEnumerator Contract(int shotPattern=-1)
        {
            StopAllCoroutines();

            Coroutine fireCoroutine = FireBehaviour(shotPattern);
            while (this != null && !IsContracted)
            {
                transform.localPosition += 
                    Vector3.ClampMagnitude(-Direction * Speed * Time.deltaTime, transform.localPosition.sqrMagnitude);
                yield return null;
            }

            if (this == null) {
                yield break;
            }

            if (fireCoroutine != null)
                StopCoroutine(fireCoroutine);
        }

        public IEnumerator CFireBehaviour(int shotPattern)
        {
            if (shotPattern == 0)
                yield return FireTowardsPlayer(bullet, GameObject.FindGameObjectWithTag("Player"));
            else if (shotPattern == 1)
                yield return FireArc(bullet);
            else if (shotPattern == 2)
                yield return FireDown(bulletPointy);
            else if (shotPattern == 3)
                yield return FireSemiCircles(bullet);
            else
                yield return null;
        }

        Coroutine FireBehaviour(int shotPattern)
        {
            switch (shotPattern)
            {
                case 0:
                    return StartCoroutine(FireTowardsPlayer(bullet, GameObject.FindGameObjectWithTag("Player")));
                case 1:
                    return StartCoroutine(FireArc(bullet));
                case 2:
                    return StartCoroutine(FireDown(bulletPointy));
                case 3:
                    return StartCoroutine(FireSemiCircles(bullet));
                default:
                case -1:
                    return null;
            }
        }

        IEnumerator FireTowardsPlayer(GameObject bullet, GameObject player)
        {
            int numberOfBullets = Parent.LerpByHealth(10, 5);
            float timeBetweenBullets = Parent.LerpByHealth(0.1f, 0.25f);
            float timeBetweenBursts = Parent.LerpByHealth(0.5f, 2.0f);
            while (true) {
                for (int i = 0; i < numberOfBullets; i++) {
                    Shoot(
                        bullet, 
                        Vector3.Angle(shootTransform.position, player.transform.position),
                        shootTransform
                    );
                    yield return new WaitForSeconds(timeBetweenBullets);
                }
                yield return new WaitForSeconds(timeBetweenBursts);
            }
        }

        IEnumerator FireDown(GameObject bullet)
        {
            float timeBetweenBullets = Parent.LerpByHealth(0.05f, 0.2f);
            while (true)
            {
                Shoot(bullet, 180f, shootTransform);
                yield return new WaitForSeconds(timeBetweenBullets);
            }
        }

        public IEnumerator FireArc(GameObject bullet)
        {
            yield return Arc(90f, 360f-90f, Distance/Speed, shootTransform, bullet, 10);
        }

        public IEnumerator FireSemiCircles(GameObject bullet) {
            float waitTime = Parent.LerpByHealth(0.25f, 0.5f);

            float angleA = -180f;
            float angleB = -90f;
            if (Direction.x > 0f) {
                angleA = 90f;
                angleB = 180f;
            }

            for (int i = 0; i < 5; i++) {
                float deltaAngle = 10f*i;
                yield return Arc(angleA-deltaAngle, angleB+deltaAngle, Distance/Speed, shootTransform, bullet, 10);
                yield return new WaitForSeconds(waitTime);
            }
        }

        public IEnumerator FireBounce()
        {
            yield return Arc(100f, 360f-100f, Distance/Speed, shootTransform, bulletBounce, 6);
        }

        protected override IEnumerator CLogic()
        {
            yield break;
        }
    }
}