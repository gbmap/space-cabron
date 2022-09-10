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
        [SerializeField] GameObject bulletCos;

        [SerializeField] Transform shootTransform;

        protected override IEnumerator CLogic()
        {
            while (true)
            {
                yield return Shake(1);
                yield return new WaitForSeconds(3f);
                yield return FireBounce();
                yield return new WaitForSeconds(3f);
                yield return WallOfBullets();
                yield return new WaitForSeconds(3f);
            }
        }


        IEnumerator ExtendFiringSimultaneous(int shotPattern)
        {
            StartCoroutine(ExtendIfExists(Left, shotPattern));
            StartCoroutine(ExtendIfExists(Right, shotPattern));
            if (Left == null && Right == null)
                yield break;
            
            while (Left != null && !Left.IsExtended)
                yield return null;
            
            while (Right != null && !Right.IsExtended)
                yield return null;
        }

        IEnumerator ContractFiringSimultaneous()
        {
            StartCoroutine(ContractIfExists(Left, -1));
            StartCoroutine(ContractIfExists(Right, -1));
            if (Left == null && Right == null)
                yield break;
            
            while (Left != null && !Left.IsContracted)
                yield return null;
            
            while (Right != null && !Right.IsContracted)
                yield return null;
        }

        IEnumerator ExtendLeftThenRight(int shotPattern=-1)
        {
            yield return ExtendIfExists(Left, shotPattern);
            yield return ExtendIfExists(Right, shotPattern);
        }

        IEnumerator ContractLeftThenRight(int shotPattern)
        {
            yield return ContractIfExists(Left, shotPattern);
            yield return ContractIfExists(Right, shotPattern);
        }

        IEnumerator ExtendRightThenLeft(int shotPattern=-1)
        {
            yield return ExtendIfExists(Right, shotPattern);
            yield return ExtendIfExists(Left, shotPattern);
        }

        IEnumerator ContractRightThenLeft(int shotPattern)
        {
            yield return ContractIfExists(Right, shotPattern);
            yield return ContractIfExists(Left, shotPattern);
        }

        IEnumerator ExtendIfExists(BossCannonNode node, int shotPattern)
        {
            if (node != null)
                yield return node.Extend(shotPattern);
        }

        IEnumerator ContractIfExists(BossCannonNode node, int shotPattern)
        {
            if (node != null)
                yield return node.Contract(shotPattern);
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

        IEnumerator Shake(BossCannonNode node, int shotPattern) 
        {
            int numberOfShakes = LerpByHealth(10, 5);
            for (int i = 0; i < numberOfShakes; i++)
            {
                yield return ExtendIfExists(node, shotPattern);
                yield return ContractIfExists(node, shotPattern);
            }
        }

        IEnumerator FireBounce()
        {
            if (Left != null)
                StartCoroutine(Left.FireBounce());
            if (Right != null)
                StartCoroutine(Right.FireBounce());
            yield return new WaitForSeconds(2f);
        }

        IEnumerator WallOfBullets()
        {
            StartCoroutine(FireTowardsSin(GameObject.FindGameObjectWithTag("Player")));
            yield return Shake(Left, 2);
            yield return Shake(Right, 2);
            yield return new WaitForSeconds(2f);
        }

        IEnumerator FireTowardsSin(GameObject player)
        {
            int numberOfBursts = LerpByHealth(10, 5);
            float angle = 0f;
            if (player != null)
            {
                angle = Vector3.Angle(
                    transform.position, player.transform.position
                );
            }

            for (int j = 0; j < numberOfBursts; j++)
            {
                int numberOfBullets = LerpByHealth(10, 5);
                for (int i = 0; i < numberOfBullets; i++)
                {
                    float alpha = Mathf.Lerp(
                        -60f, 60f, 
                        ((float)(i+j)/(numberOfBullets+numberOfBursts))
                    );
                    Shoot(bulletSin, angle + alpha, shootTransform);
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }
    }
}