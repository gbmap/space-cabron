using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public class MoveTowardsRandomEnemy : MonoBehaviour
    {
        Vector3 velocity;
        Vector3 acceleration;

        Transform target;
        float timer;

        void Start()
        {
            FindTarget();
            velocity = new Vector3((Random.value-0.5f), (Random.value-0.5f)*0.1f, 0f);
            timer = 0f;
        }

        void FindTarget() {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
            if (targets.Length == 0) {
                return;
            }

            GameObject randomTarget = targets[Random.Range(0, targets.Length)];
            target = randomTarget.transform;
        }

        void Update()
        {
            if (target == null) {
                FindTarget();
            }

            Vector3 delta = Vector3.up*5f;
            if (target != null) {
                delta = target.position - transform.position;
            }
            acceleration = Vector3.ClampMagnitude(delta, Mathf.Lerp(0f, 20f, timer/0.5f));
            velocity += acceleration * Time.deltaTime;
            velocity -= Vector3.ClampMagnitude(velocity, 1f)*0.2f*Time.deltaTime;
            transform.position += velocity*Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f,0f,Vector3.Angle(Vector3.up, velocity.normalized));

            timer += Time.time;
        }
    }
}