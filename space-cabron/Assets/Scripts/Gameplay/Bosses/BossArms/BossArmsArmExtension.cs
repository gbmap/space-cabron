using UnityEngine;

namespace SpaceCabron.Gameplay.Bosses
{
    public class BossArmsArmExtension : MonoBehaviour
    {
        [SerializeField] Transform shoulder;
        [SerializeField] Transform hand;

        void Update()
        {
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
        }
    }
}