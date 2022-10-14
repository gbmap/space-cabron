using System.Collections;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossCannons2MainCannonBehaviour : MonoBehaviour
    {
        public GameObject BulletPingPong;
        public ShotPattern[] ShotPatterns;

        public void FireBigPingPong() {
            var instance = Instantiate(
                BulletPingPong,
                transform.position,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
            );
            instance.transform.localScale = Vector3.one * 5f;
        }

        public IEnumerator FireRandomPattern() {
            // Disable all shot patterns
            foreach (var pattern in ShotPatterns) {
                pattern.enabled = false;
            }

            // Select number of random patterns to enable
            int numPatterns = Mathf.Min(
                Random.Range(1, ShotPatterns.Length+1), ShotPatterns.Length
            );

            // Select random pattern indexes
            int[] patternIndexes = new int[numPatterns];
            for (int i = 0; i < numPatterns; i++) {
                int index = Random.Range(0, ShotPatterns.Length);
                patternIndexes[i] = index;
            }

            // Enable shot patterns
            foreach (var index in patternIndexes) {
                ShotPatterns[index].enabled = true;
            }

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Disable shot patterns
            foreach (var index in patternIndexes) {
                ShotPatterns[index].enabled = false;
            }
        }
    }
}