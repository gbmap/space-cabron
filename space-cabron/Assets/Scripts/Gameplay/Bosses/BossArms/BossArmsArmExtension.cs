using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossArmsArmExtension : MonoBehaviour
    {
        [SerializeField] Transform shoulder;
        [SerializeField] Transform hand;
        [SerializeField] bool adjustRotation;

        void Update()
        {
            if (shoulder == null || hand == null) {
                Destroy(gameObject);
                return;
            }

            Vector3 a = shoulder.transform.position;
            Vector3 b = hand.transform.position;

            Vector3 targetPosition = (a+b)/2f;
            float targetScale = Vector3.Distance(b,a);

            transform.position = targetPosition;
            transform.localScale = new Vector3(
                transform.localScale.x, 
                targetScale/(32f/100f), 
                transform.localScale.z
            );

            if (adjustRotation)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(Vector3.up, b-a, Vector3.forward));
            }
        }
    }
}