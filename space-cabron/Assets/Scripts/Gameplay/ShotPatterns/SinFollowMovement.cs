using System.Linq;
using UnityEngine;

namespace Gmap
{
    public class SinFollowMovement : MonoBehaviour
    {
        public AnimationCurve FollowTargetCurve;
        public float Speed = 1f;
        public float RotationSpeed = 30f;
        public float SinAmplitude = 20f;
        public float SinFrequency = 1f;

        float awakenTime;
        new Rigidbody2D rigidbody;
        Transform targetPlayer;

        float T { get { return Mathf.Max(0f, Time.time - awakenTime); } }
        float FollowFactor { get { return FollowTargetCurve.Evaluate(T); } }

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            FollowTargetCurve.postWrapMode = WrapMode.Clamp;
            FollowTargetCurve.postWrapMode = WrapMode.Clamp;
        }

        void Start()
        {
        }

        void OnEnable()
        {
            awakenTime = Time.time;
            targetPlayer = GameObject.FindGameObjectsWithTag("Player")
                                     .OrderBy(p => Vector3.Distance(p.transform.position, transform.position))
                                     .FirstOrDefault()?
                                     .transform;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            UpdateRotation();

            Vector2 p = new Vector2(
                rigidbody.position.x + transform.up.x*Speed*Time.fixedDeltaTime,
                rigidbody.position.y + transform.up.y*Speed*Time.fixedDeltaTime
            );
            rigidbody.MovePosition(p);
        }

        void UpdateRotation()
        {
            if (targetPlayer == null) {
                return;
            }

            Vector3 delta = (targetPlayer.transform.position - transform.position).normalized;
            float targetAngle = 180f
                              + Vector3.SignedAngle(Vector3.down, delta, Vector3.forward)
                              + Mathf.Sin(Time.time * SinFrequency * Mathf.PI)*SinAmplitude;
            targetAngle = Mathf.Lerp(rigidbody.rotation, targetAngle, FollowFactor);
            float deltaAngle    = Mathf.Clamp(targetAngle - rigidbody.rotation, -1f, 1f)*RotationSpeed;
            rigidbody.rotation += deltaAngle * Time.fixedDeltaTime;


            // transform.rotation = Quaternion.RotateTowards(
            //     transform.rotation, 
            //     Quaternion.LookRotation(delta, -Vector3.forward), 
            //     RotationSpeed * Time.fixedDeltaTime
            // );
        }
    }
}