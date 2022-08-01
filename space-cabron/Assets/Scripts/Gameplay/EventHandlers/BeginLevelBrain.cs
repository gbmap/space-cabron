using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class BeginLevelBrain : AnimationBrain
    {
        protected override IEnumerator AnimationCoroutine()
        {
            Vector3 originalPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, -0.25f, 0f)
            );
            originalPosition.z = 0f;
            transform.position = originalPosition;

            Vector3 targetPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(0.5f, 0.25f, 0f)
            );
            targetPosition.z = 0f;
            yield return MoveTowardsPosition(targetPosition);
            yield return new WaitForSeconds(1f);
        }
    }
}