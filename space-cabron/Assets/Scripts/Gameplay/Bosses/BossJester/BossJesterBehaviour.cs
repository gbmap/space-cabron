using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossJesterBehaviour : BossBehaviour
    {
        public GameObject[] ShootPatterns;

        protected override IEnumerator CLogic() {
            while (true) {
                int numberOfPatterns = LerpByHealth(10, 3);
                SelectRandomPattern(numberOfPatterns);
                yield return new WaitForSeconds(LerpByHealth(2f, 4f));
                DisableShooting();
                yield return new WaitForSeconds(LerpByHealth(0.5f, 2f));
            }
        }

        private void DisableShooting()
        {
            System.Array.ForEach(ShootPatterns, p => p.SetActive(false));
        }

        private void SelectRandomPattern(int nPatterns)
        {
            nPatterns = Mathf.Clamp(nPatterns, 1, ShootPatterns.Length);

            DisableShooting();
            int[] indexes = Enumerable.Range(0, ShootPatterns.Length)
                                      .OrderBy(x=>UnityEngine.Random.value)
                                      .Take(nPatterns)
                                      .ToArray();
            for (int i = 0; i < indexes.Length; i++) {
                ShootPatterns[indexes[i]].SetActive(true);
            }
        }

        protected IEnumerator MoveToRandomPosition() {
            yield break;
        }
    }
}