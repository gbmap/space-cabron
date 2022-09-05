using System.Collections;
using System.Collections.Generic;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Bosses;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class BossIntroBrain : AnimationBrain
    {
        protected override IEnumerator AnimationCoroutine()
        {
            Vector3 targetPosition = Vector3.up * 2f;
            Vector3 direction = targetPosition - transform.position; 
            while (Vector3.Distance(transform.position, targetPosition) >= 0.05f)
            {
                float d = Vector3.Distance(transform.position, targetPosition);
                input.Movement = Vector3.ClampMagnitude(direction, Mathf.Min(1f, d));
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            System.Array.ForEach(
                GetComponentsInChildren<BossBehaviour>(), b => b.StartLogic()
            );
        }
    }
}