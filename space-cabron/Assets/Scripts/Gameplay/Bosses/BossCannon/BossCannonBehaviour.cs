using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossCannonBehaviour : BossBehaviour
    {
        [SerializeField] BossCannonNode Left;
        [SerializeField] BossCannonNode Right;

        [SerializeField] GameObject bullet;
        [SerializeField] GameObject bulletSin;
        [SerializeField] GameObject bulletCos;

        [SerializeField] Transform shootTransform;

        protected override IEnumerator CLogic()
        {
            while (true)
            {
                // yield return WallOfBullets();
                // yield return new WaitForSeconds(3f);
                float waitTime = LerpByHealth(0.5f, 1f);
                yield return Shake(1);
                yield return new WaitForSeconds(waitTime);
                yield return FireBounce();
                yield return new WaitForSeconds(waitTime);
                yield return Shake(2);
                yield return new WaitForSeconds(waitTime);
                yield return FireBounce();
                yield return new WaitForSeconds(waitTime);
                yield return WallOfBullets();
                yield return new WaitForSeconds(waitTime);
                yield return FireArcs();
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

        IEnumerator FireIfExists(BossCannonNode node, int shotPattern) 
        {
            if (node != null)
                yield return node.CFireBehaviour(shotPattern);
        }

        ////////////
        // BEHAVIOURS
        IEnumerator Shake(int shotPattern) 
        {
            int numberOfShakes = LerpByHealth(10, 5);
            float waitTime = LerpByHealth(0f, 0.5f);
            var node = Left;
            for (int i = 0; i < numberOfShakes; i++)
            {
                yield return ExtendIfExists(node, shotPattern);
                yield return new WaitForSeconds(waitTime);
                yield return ContractIfExists(node, shotPattern);
                yield return new WaitForSeconds(waitTime);
                node = node == Left ? Right : Left;
            }
        }

        IEnumerator Shake(BossCannonNode node, int shotPattern) 
        {
            int numberOfShakes = LerpByHealth(10, 5);
            float waitTime = LerpByHealth(0f, 1f);
            for (int i = 0; i < numberOfShakes; i++)
            {
                yield return ExtendIfExists(node, shotPattern);
                yield return new WaitForSeconds(waitTime);
                yield return ContractIfExists(node, shotPattern);
                yield return new WaitForSeconds(waitTime);
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

        IEnumerator FireArcs()
        {
            float waitTime = LerpByHealth(0.25f, 1f);
            var node = Left;
            for (int i = 0; i < 2; i++) {
                yield return ExtendIfExists(node, -1);
                yield return new WaitForSeconds(waitTime);
                yield return FireIfExists(node, 3);
                yield return new WaitForSeconds(waitTime);
                node = Right;
            }
        }

        IEnumerator WallOfBullets()
        {
            StartCoroutine(FireTowardsSin(GameObject.FindGameObjectWithTag("Player")));
            // Coroutine shakeCoroutine = StartCoroutine(Shake(Left, 2));
            // Coroutine fireCoroutine = StartCoroutine(FireIfExists(Right, 0));
            yield return Shake(Left, 2);
            yield return Shake(Right, 2);
            yield return new WaitForSeconds(2f);
        }

        IEnumerator FireTowardsSin(GameObject player)
        {
            int numberOfBursts = LerpByHealth(10, 5);
            float angle = 0f;

            for (int j = 0; j < numberOfBursts; j++)
            {
                int numberOfBullets = LerpByHealth(10, 5);
                var bullet = bulletSin;
                float timeOffset = j%2==0?0f:Mathf.PI;
                float timeBetweenBullets = LerpByHealth(0.1f, 0.5f);
                for (int i = 0; i < numberOfBullets; i++)
                {
                    if (player != null)
                    {
                        angle = Vector3.SignedAngle(
                            shootTransform.position, 
                            player.transform.position-shootTransform.position,
                            Vector3.forward
                        ) + Vector3.Angle(shootTransform.up, Vector3.up);
                        // angle = 0f;
                    }
                    else
                        player = GameObject.FindGameObjectWithTag("Player");

                    float alpha = 0f;
                    var bulletInstance = Shoot(bullet,  angle + alpha, shootTransform);
                    bulletInstance.GetComponent<SinMovement>().TimeOffset = timeOffset;

                    Shoot(this.bullet, angle+alpha, shootTransform);
                    yield return new WaitForSeconds(timeBetweenBullets);
                }
            }
        }
    }
}