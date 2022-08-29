using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Brain/Movement Strategy/Loop Sideways")]
    public class ScriptableLoopSidewaysMovementStrategy : ScriptableMovementStrategy2D
    {
        static GameObject player;
        static GameObject Player
        {
            get
            {
                if (player == null)
                    player = GameObject.FindGameObjectWithTag("Player");
                return player;
            }
        }

        public float SteerSpeed = 1f;
        public float Width = 8f;
        public override Vector2 GetDirection(MovementStrategyArgs args)
        {
            Vector2 pos = args.Object.transform.position;
            Vector2 vel = pos - new Vector2(args.LastPosition.x, args.LastPosition.y);

            Vector2 targetPos = GetDesiredPosition(pos, vel);
            Vector2 delta = (targetPos-pos);
            if (delta.sqrMagnitude <= 0.001f)
                return Vector2.left;

            float distanceFactor = Vector2.Distance(pos, targetPos)+0.05f;
            Vector3 final = delta.normalized * SteerSpeed * distanceFactor;
            return Vector3.ClampMagnitude(final, 1f);
        }

        private Vector2 LerpVelocity(Vector2 velocity, Vector2 desired, float speed)
        {
            return Vector2.Lerp(velocity, desired, Time.deltaTime*speed);
        }

        private Vector2 GetDesiredPosition(Vector2 pos, Vector2 vel)
        {
            if (Camera.main == null)
                return Vector2.zero; 

            Vector3 up = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.75f, 0f));
            Vector2 a = Camera.main.transform.position 
                      + Vector3.left*Width/2f
                      + Vector3.up*(up.y-Camera.main.transform.position.y);
            Vector2 b = Camera.main.transform.position 
                      + Vector3.right*Width/2f
                      + Vector3.up*(up.y-Camera.main.transform.position.y);
            if (vel.x < 0f)
            {
                if (pos.x < a.x)
                    return b;
                return a;
            }
            else
            {
                if (pos.x > b.x)
                    return a;
                return b;
            }
        }
    }
}