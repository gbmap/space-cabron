using System.Collections;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses {
    public class BossCannons2CannonBehaviour : MonoBehaviour {
        public Transform LaserBase;
        public Transform LaserEnd;
        public Transform Laser;

        private AnimationCurve laserScaleCurve;
        void Awake() {
            laserScaleCurve = new AnimationCurve();
            laserScaleCurve.AddKey(new Keyframe(0f, 0f));
            laserScaleCurve.AddKey(new Keyframe(0.25f, 0.1f));
            laserScaleCurve.AddKey(new Keyframe(0.75f, 0f, 0f, 0.5f));
            laserScaleCurve.AddKey(new Keyframe(1f, 1f, 0.5f, 0f));
            laserScaleCurve.preWrapMode = WrapMode.ClampForever;
            laserScaleCurve.postWrapMode = WrapMode.ClampForever;
        }

        public IEnumerator Fire() {
            LaserEnd.position = transform.position
                              - transform.up * 100f;

            float t = 0f;
            Transform laser = Laser;
            Collider laserCollider = laser.GetComponent<Collider>();
            while (t < 1f) {
                if (laser == null) {
                    yield break;
                }
                t += Time.deltaTime * 2f;
                laser.localScale = new Vector3(
                    laserScaleCurve.Evaluate(t)*0.9f,
                    laser.localScale.y,
                    laser.localScale.z
                );

                if (laserCollider)
                    laserCollider.enabled = t > 0.75f;

                yield return null;
            }
        }

        public IEnumerator StopFiring() {
            float t = 1f;
            Transform laser = Laser;
            Collider laserCollider = laser.GetComponent<Collider>();
            while (t > 0f) {
                if (laser == null) {
                    yield break;
                }
                t = Mathf.Clamp(t - Time.deltaTime * 2f, 0f, 1f);
                laser.localScale = new Vector3(
                    laserScaleCurve.Evaluate(t)*0.9f,
                    laser.localScale.y,
                    laser.localScale.z
                );

                if (laserCollider)
                    laserCollider.enabled = t > 0.75f;

                yield return null;
            }
            LaserEnd.position = transform.position;

        }
    }
}