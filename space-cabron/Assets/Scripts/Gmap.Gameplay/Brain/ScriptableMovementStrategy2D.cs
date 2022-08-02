using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    public class MovementStrategyArgs
    {
        public GameObject Object;
    }

    public abstract class ScriptableMovementStrategy<T> : ScriptableObject
    {
        public abstract T GetDirection(MovementStrategyArgs args);
    }

    public abstract class ScriptableMovementStrategy2D : ScriptableMovementStrategy<Vector2> {}
}
