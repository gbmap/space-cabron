using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public abstract class BossBehaviour : MonoBehaviour
    {
        public bool IsRunning { get; private set; }

        public void StartLogic() { 
            IsRunning = true;
            StartCoroutine(CLogic()); 
        }
        protected abstract IEnumerator CLogic();

        protected ColorHealth[] healths;

        protected virtual void Awake()
        {
            healths = GetComponentsInChildren<ColorHealth>();
        }

        protected float HealthPercentage
        {
            get {
                float totalHealth = 0;
                float totalMaxHealth = 0;
                foreach (var health in healths)
                {
                    totalHealth += health.CurrentHealth;
                    totalMaxHealth += health.MaxHealth;
                }
                return totalHealth / totalMaxHealth;
            }
        }

        protected float LerpByHealth(float min, float max)
        {
            return Mathf.Lerp(min, max, HealthPercentage);
        }

        protected int LerpByHealth(int min, int max)
        {
            return Mathf.RoundToInt(Mathf.Lerp(min, max, HealthPercentage));
        }

        protected GameObject Shoot(
            GameObject bullet,
            float angle, 
            Transform gunTransform
        ) {
            var instance = Instantiate(
                bullet, 
                gunTransform.position,
                Quaternion.Euler(0f, 0f, angle) * gunTransform.localRotation
            );
            return instance;
        }


        protected IEnumerator Arc(float targetAngle,
                        float startingAngle,
                        float totalTime,
                        Transform shootTransform,
                        GameObject bullet,
                        int nShots
        ) {
            // float totalTime = Distance/Speed;

            float angle = startingAngle;
            int numberOfShots = nShots;
            float angleIncrement = (targetAngle - angle) / (numberOfShots-1);
            for (int i = 0; i < numberOfShots; i++)
            {
                Shoot(bullet, angle, shootTransform);
                angle += angleIncrement;
                yield return new WaitForSeconds(totalTime / (numberOfShots+2));
            }
        }
    }
}