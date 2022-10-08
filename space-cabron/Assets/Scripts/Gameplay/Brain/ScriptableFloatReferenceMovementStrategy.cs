using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

[CreateAssetMenu(menuName="Space Cabrón/Brain/Movement Strategy/Reference")]
public class ScriptableFloatReferenceMovementStrategy : ScriptableMovementStrategy2D
{
    public FloatBusReference X;
    public FloatBusReference Y;

    public override Vector2 GetDirection(MovementStrategyArgs args)
    {
        return new Vector2(X.Value, Y.Value);
    }
}
