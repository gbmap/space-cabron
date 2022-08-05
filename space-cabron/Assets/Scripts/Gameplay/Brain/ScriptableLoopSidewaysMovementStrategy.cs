using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Brain/Movement Strategy/Loop Sideways")]
    public class ScriptableLoopSidewaysMovementStrategy : ScriptableMovementStrategy2D
    {
        public override Vector2 GetDirection(MovementStrategyArgs args)
        {
            Vector3 a = Camera.main.ViewportToWorldPoint(new Vector3(0.15f, 0, 0));
            Vector3 b = Camera.main.ViewportToWorldPoint(new Vector3(0.85f, 0, 0));
            float t = Mathf.Sin(Time.time)*0.5f+0.5f;
            Vector3 pos = Vector3.Lerp(a, b, t);
            float x = (args.Object.transform.position - pos).normalized.x;
            return new Vector2(x, 0f);
        }
    }
}