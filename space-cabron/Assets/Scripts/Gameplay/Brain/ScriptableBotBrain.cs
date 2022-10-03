using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay {
    public class ScriptableBotBrain : ScriptableBrain<InputState>
    {
        public override InputState GetInputState(InputStateArgs args)
        {
            GameObject[] enemies = GameObject.FindObjectsOfType<ColorHealth>()
                .Where(h=>h.CanTakeDamage && h.tag != "Player" && h.tag != "Drone")
                .Select(h=>h.gameObject)
                .ToArray();

            var closestEnemy = enemies.OrderBy(
                e => Mathf.Abs(args.Object.transform.position.x - e.transform.position.x)
            )
            .OrderBy(e=>e.transform.position.y)
            .Where(
                e=>e.GetComponent<ColorHealth>().CanTakeDamage 
                && e.transform.position.y > args.Object.transform.position.y
            ).FirstOrDefault();

            if (closestEnemy == null) {
                return new InputState {
                    Movement = SteerAwayFromBullets(args.Object),
                    Shoot = false,
                    Pause = false,
                    Color = EColor.None,
                };
            }

            var color = closestEnemy.GetComponent<ColorHealth>().CurrentColor;
            if (!args.Object.GetComponent<Fire>().WaitingForPress) {
                color = EColor.None;
            }

            Vector2 awayFromBullets = SteerAwayFromBullets(args.Object);
            float xTowardsEnemy = closestEnemy.transform.position.x - args.Object.transform.position.x;
            xTowardsEnemy = closestEnemy.GetComponentInChildren<Collider2D>().bounds.center.x - args.Object.transform.position.x;
            xTowardsEnemy = Mathf.Sign(xTowardsEnemy) * Mathf.Clamp01(Mathf.Abs(xTowardsEnemy));

            Vector2 movement = Vector3.Normalize(
                awayFromBullets*4f 
             +  -new Vector2(args.Object.transform.position.x, args.Object.transform.position.y)*awayFromBullets.magnitude 
             +  Vector2.right*xTowardsEnemy
            );
            return new InputState {
                Movement = movement,
                Pause = false,
                Shoot = color != EColor.None,
                Color = color,
            };
        }

        Vector2 SteerAwayFromBullets(GameObject player) {
            return Vector2.zero;
            var bullets = Physics2D.OverlapCircleAll(player.transform.position, 0.5f, LayerMask.GetMask("EnemyBullet"));
            Vector2 movement = Vector2.zero;
            for (int i = 0; i < bullets.Length; i++) {
                var bullet = bullets[i];
                var direction = (bullet.transform.position - player.transform.position).normalized;
                movement -= new Vector2(direction.x, direction.y);
            }
            return Vector3.ClampMagnitude(movement, 1f);
        }
    }
}