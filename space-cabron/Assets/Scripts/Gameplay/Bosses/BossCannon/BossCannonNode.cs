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
                default:
                case -1:
                    return null;
            }
        }

        IEnumerator FireTowardsPlayer(GameObject bullet, GameObject player)
        {
            yield break;
        }

        IEnumerator FireDown(GameObject bullet)
        {
            while (true)
            {
                Shoot(bullet, 180f, shootTransform);
                yield return new WaitForSeconds(0.05f);
            }
        }

        IEnumerator FireArc(GameObject bullet)
        {
            yield return Arc(90f, 360f-90f, Distance/Speed, shootTransform, bullet, 10);
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