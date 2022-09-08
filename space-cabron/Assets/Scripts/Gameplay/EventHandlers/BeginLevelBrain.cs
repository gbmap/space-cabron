using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class BeginLevelBrain : AnimationBrain
    {
        public int Index = 0;
        public int MaxPlayers = 2;

        protected override IEnumerator AnimationCoroutine()
        {
            float halfWidth = 0.25f;
            float x = Mathf.Lerp(0.5f-halfWidth, 0.5f+halfWidth, ((float)Index)/(MaxPlayers-1));

            Vector3 originalPosition = Vector3.down * 10f;
            if (Camera.main != null)
            {
                originalPosition = Camera.main.ViewportToWorldPoint(
                    new Vector3(x, -0.25f, 0f)
                );
            }
            originalPosition.z = 0f;
            transform.position = originalPosition;

            Vector3 targetPosition = Vector3.zero;
            if (Camera.main != null)
            {
                targetPosition = Camera.main.ViewportToWorldPoint(
                    new Vector3(x, 0.25f, 0f)
                );
            }
            targetPosition.z = 0f;
            yield return MoveTowardsPosition(targetPosition);
            yield return new WaitForSeconds(1f);
        }
    }
}