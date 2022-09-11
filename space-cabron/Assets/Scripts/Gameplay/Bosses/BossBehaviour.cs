using System;
using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public abstract class BossBehaviour : MonoBehaviour
    {
        protected BossBehaviour Parent;

        public bool IsRunning { get; private set; }

        public void StartLogic() { 
            IsRunning = true;
            StartCoroutine(CLogic()); 
        }
        protected abstract IEnumerator CLogic();

        protected ColorHealth[] healths;
        ColorHealth mainHealth;

        protected virtual void Awake()
        {
            if (transform.parent != null)
                Parent = transform.parent.GetComponentInParent<BossBehaviour>();
            healths = GetComponentsInChildren<ColorHealth>();
            mainHealth = GetComponent<ColorHealth>();
            mainHealth.OnDestroy += Callback_OnDestroyed;
        }

        public void EnableColorHealth()
        {
            mainHealth.CanTakeDamage = true;
        }

        private void Callback_OnDestroyed(MsgOnObjectDestroyed obj)
        {
            StopAllCoroutines();
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

        public float LerpByHealth(float min, float max)
        {
            return Mathf.Lerp(min, max, HealthPercentage);
        }

        public int LerpByHealth(int min, int max)
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
                Quaternion.Euler(0f, 0f, angle+gunTransform.eulerAngles.z) 
            );
            return instance;
        }


        protected IEnumerator Arc(
            float targetAngle,
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