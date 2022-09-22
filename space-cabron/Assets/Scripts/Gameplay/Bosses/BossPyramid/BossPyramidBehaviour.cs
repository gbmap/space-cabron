using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses {
    public class BossPyramidBehaviour : BossBehaviour
    {
        public GameObject PyramidUpperLeft;
        public GameObject PyramidUpperRight;
        public GameObject PyramidLowerLeft;
        public GameObject PyramidLowerRight;

        public GameObject PyramidRoots;

        Dictionary<Transform, Vector3> pyramidToPosition;

        public float PyramidSpeed = 5f;
        public float RotationSpeed = 180f;
        public float RotationTimeFrequency = 1f;
        public float RotationPiFrequency = 1f;
        public Vector3 RotationAxis = Vector3.forward;

        public class RotationConfig {
            public float RotationSpeed = 0f;
            public float RotationTimeFrequency = 1f;
            public float RotationPiFrequency = 1f;
            public float MaxExtension = 0.25f;
            public Vector3 RotationAxis = Vector3.forward;
            public RotationConfig(
                float rotationSpeed,
                float rotationTimeFrequency, 
                float rotationPiFrequency,
                float maxExtension,
                Vector3 rotationAxis
            ) {
                this.RotationSpeed = rotationSpeed;
                this.RotationTimeFrequency = rotationTimeFrequency;
                this.RotationPiFrequency = rotationPiFrequency;
                this.MaxExtension = maxExtension;
                this.RotationAxis = rotationAxis;
            }
        }

        public float SinExtension = 0.25f;

        public float[] PyramidsExtension = new float[4] { 0f, 0f, 0f, 0f };
        public System.Func<float, int, float>[] PyramidExtensionFunctions = new System.Func<float, int, float>[4];

        Vector3[] PyramidsInitialPositions = new Vector3[4];

        protected override void Awake() {
            pyramidToPosition = new Dictionary<Transform, Vector3>() {
                { PyramidUpperLeft.transform,  Vector3.up + Vector3.left },
                { PyramidUpperRight.transform, Vector3.up + Vector3.right },
                { PyramidLowerLeft.transform,  Vector3.down + Vector3.left },
                { PyramidLowerRight.transform, Vector3.down + Vector3.right }
            };

            for (int i = 0; i < 4; i++) {
                PyramidExtensionFunctions[i] = ConstExtensionFunc;
            }

            for (int i = 0; i < 4; i++) {
                PyramidsInitialPositions[i] = GetPyramid(i).localPosition;
            }

            base.Awake();
        }

        private Transform GetPyramid(int i)
        {
            return pyramidToPosition.Keys.ToArray()[i].transform;
            
        }

        void Update() {
            // int i = 0;
            for (int i = 0; i < pyramidToPosition.Keys.Count; i++) {
            // foreach (var pyramid in pyramidToPosition.Keys) {
                var pyramid = GetPyramid(i);
                if (pyramid  == null){
                    continue;
                }
                pyramid.transform.localPosition = Vector3.MoveTowards(
                    pyramid.transform.localPosition,
                    PyramidsInitialPositions[i] 
                  + pyramidToPosition[pyramid] 
                  * PyramidsExtension[i],
                    PyramidSpeed * Time.deltaTime
                );
            }

            float t = Time.time * RotationTimeFrequency;
            for (int j = 0; j < 4; j++) {
                PyramidsExtension[j] = PyramidExtensionFunctions[j](t, j);
            }

            PyramidRoots.transform.Rotate(RotationAxis, RotationSpeed * Time.deltaTime);
        }

        private float SinExtensionFunc(float t, int pyramidIndex) {
            float tOffset = Mathf.PI * RotationPiFrequency * pyramidIndex;
            float sin = Mathf.Sin(t+tOffset);
            float extension = (sin+1f)*0.5f*SinExtension;
            return extension;
        }

        private float ConstExtensionFunc(float t, int pyramidIndex) {
            return SinExtension;
        }

        protected override IEnumerator CLogic()
        {
            while (true)
            {
                // yield return RotateUpAxis(2);
                // yield return new WaitForSeconds(10f);
                // yield return ResetRotation();

                for (int i = 0; i < 2; i++) {
                    SetRotationConfig(new RotationConfig(90f, 1f, 1f, 0.25f, Vector3.forward), new System.Func<float, int, float>[] {
                        ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc
                    });
                    yield return new WaitForSeconds(10f);

                    SetRotationConfig(new RotationConfig(45f, 2.0f, 2f, 0.5f, Vector3.forward), new System.Func<float, int, float>[] {
                        SinExtensionFunc,ConstExtensionFunc,ConstExtensionFunc,SinExtensionFunc
                    });
                    yield return new WaitForSeconds(10f);
                    SetRotationConfig(new RotationConfig(45f, 4.0f, 0.5f, 0.5f, Vector3.forward), new System.Func<float, int, float>[] {
                        SinExtensionFunc,SinExtensionFunc,SinExtensionFunc,SinExtensionFunc
                    });
                    yield return new WaitForSeconds(10f);

                    StandStill();
                    yield return ResetRotation();
                    // yield return new WaitForSeconds(10f);
                    RotationSpeed = 45f;
                    yield return RotateUpAxis(1);
                }

                // SetRotationConfig(new RotationConfig(90f, 4.0f, 0.5f, 0.5f, Vector3.up), new System.Func<float, int, float>[] {
                //     ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc
                // });
                // yield return new WaitForSeconds(10f);
            }
            // yield return CExtendPyramid(PyramidUpperLeft, Vector2.one, 2f);
            // yield return CExtendPyramid(PyramidUpperRight, Vector2.one, 2f);
            // yield return CExtendPyramid(PyramidLowerLeft, Vector2.one, 2f);
            // yield return CExtendPyramid(PyramidLowerRight, Vector2.one, 2f);
            // yield return RotatePyramids(100);
        }

        private IEnumerator RotateUpAxis(int halfRotations)
        {
            float speedCache = RotationSpeed;
            SetRotationConfig(new RotationConfig(0f, 4.0f, 0.5f, 0.25f, Vector3.up), new System.Func<float, int, float>[] {
                ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc
            });
            yield return RotatePyramids(halfRotations, speedCache);
        }

        private IEnumerator ResetRotation()
        {
            SetConstantFiring(false);
            SetRotationConfig(new RotationConfig(0f, 4.0f, 0.5f, 0.25f, Vector3.forward), new System.Func<float, int, float>[] {
                ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc
            });
            yield return new WaitForSeconds(1f);

            float t = 0f;
            float totalTime = 2f;
            while (true)
            {
                float angle = Mathf.Lerp(PyramidRoots.transform.eulerAngles.x, 0f, t/totalTime);
                PyramidRoots.transform.rotation = Quaternion.Euler(
                    PyramidRoots.transform.eulerAngles.x,
                    PyramidRoots.transform.eulerAngles.y,
                    angle
                );
                if (t >= totalTime+0.1f) {
                    break;
                }
                yield return null;
                t += Time.deltaTime;
            }
            yield return new WaitForSeconds(1f);
            SetConstantFiring(true);
        }

        private void StandStill()
        {
            SetRotationConfig(new RotationConfig(0f, 4.0f, 0.5f, 0.5f, Vector3.forward), new System.Func<float, int, float>[] {
                ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc,ConstExtensionFunc
            });
        }

        protected IEnumerator RotatePyramids(int halfRotations, float speed) {
            float totalAngle = 180f*halfRotations;
            while (totalAngle > 0) {
                float rotationSpeed = LerpByHealth(speed*2f, speed);
                PyramidRoots.transform.Rotate(RotationAxis, Time.deltaTime*rotationSpeed);
                totalAngle -= Time.deltaTime*rotationSpeed;
                yield return null;
            }
        }

        protected void SetRotationConfig(RotationConfig config) {
            RotationSpeed = config.RotationSpeed;
            RotationTimeFrequency = config.RotationTimeFrequency;
            RotationPiFrequency = config.RotationPiFrequency;
            SinExtension = config.MaxExtension;
            RotationAxis = config.RotationAxis;
        }

        protected void SetRotationConfig(RotationConfig config, System.Func<float, int, float>[] pyramidExtensionFunctions) {
            SetRotationConfig(config);
            PyramidExtensionFunctions = pyramidExtensionFunctions;
        }

        void SetConstantFiring(bool v) {
            ConstantShot[] shots = GetComponentsInChildren<ConstantShot>();
            System.Array.ForEach(shots, s => s.enabled = v);
        }
    }
}