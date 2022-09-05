using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossCannonBehaviour : BossBehaviour
    {
        [SerializeField] BossCannonNode Left;
        [SerializeField] BossCannonNode Right;

        [SerializeField] GameObject bulletSin;

        void Start()
        {
            StartCoroutine(CLogic());
        }

        IEnumerator CLogic()
        {
            while (true)
            {
                yield return Shake(1);
                yield return new WaitForSeconds(2f);
                yield return FireBounce();
                yield return new WaitForSeconds(2f);
                yield return WallOfBullets();
                yield return new WaitForSeconds(2f);
            }
        }


        IEnumerator ExtendFiringSimultaneous(int shotPattern)
        {
            StartCoroutine(Left.Extend(shotPattern));
            StartCoroutine(Right.Extend(shotPattern));
            while (!Left.IsExtended && !Right.IsExtended)
                yield return null;
        }

        IEnumerator ContractFiringSimultaneous()
        {
            StartCoroutine(Left.Contract());
            StartCoroutine(Right.Contract());
            while (!Left.IsContracted && !Right.IsContracted)
                yield return null;
        }

        IEnumerator ExtendLeftThenRight(int shotPattern=-1)
        {
            yield return Left.Extend(shotPattern);
            yield return Right.Extend(shotPattern);
        }

        IEnumerator ContractLeftThenRight(int shotPattern)
        {
            yield return Left.Contract(shotPattern);
            yield return Right.Contract(shotPattern);
        }

        IEnumerator ExtendRightThenLeft(int shotPattern=-1)
        {
            yield return Right.Extend(shotPattern);
            yield return Left.Extend(shotPattern);
        }

        IEnumerator ContractRightThenLeft(int shotPattern)
        {
            yield return Right.Contract(shotPattern);
            yield return Left.Contract(shotPattern);
        }

        ////////////
        // BEHAVIOURS

        IEnumerator Shake(int shotPattern)
        {
            int numberOfShakes = LerpByHealth(10, 5);
            for (int i = 0; i < numberOfShakes; i++)
            {
                yield return ExtendLeftThenRight(shotPattern);
                yield return ContractLeftThenRight(shotPattern);
            }
        }

        IEnumerator FireBounce()
        {
            StartCoroutine(Left.FireBounce());
            StartCoroutine(Right.FireBounce());
            yield return new WaitForSeconds(2f);
        }

        IEnumerator WallOfBullets()
        {
            StartCoroutine(Left.Extend(2));
            StartCoroutine(Right.Extend(2));
            StartCoroutine(FireTowards(GameObject.FindGameObjectWithTag("Player")));
            yield return new WaitForSeconds(2f);
        }

        IEnumerator FireTowards(GameObject player)
        {
            float angle = 180f;
            if (player != null)
            {
                angle = Vector3.Angle(
                    transform.position, player.transform.position
                );
            }

            for (int i = 0; i < 5; i++)
            {
                Shoot(bulletSin, angle + i * 10, transform);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}