using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    [CreateAssetMenu(menuName="Space Cabrón/Brain/Movement Strategy/Constant")]
    public class ScriptableConstantMovementStrategy : ScriptableMovementStrategy2D
    {
        public Vector2 Direction = Vector2.down;
        public override Vector2 GetDirection(MovementStrategyArgs args)
        {
            return Direction;
        }
    }
}