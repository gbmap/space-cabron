using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.ScriptableReferences;
using Gmap.Gameplay;

namespace SpaceCabron.Gameplay
{

    [CreateAssetMenu(menuName="Space Cabrón/Brain/Scriptable Brain")]
    public class ScriptableSpaceCabronBrain : ScriptableAIBrain<InputState>
    {
        public override InputState GetInputState(InputStateArgs args)
        {
            return new InputState
            {
                Movement = MovementStrategy.GetDirection(new MovementStrategyArgs
                {
                    Object = args.Object
                }),
                Shoot = FiringStrategy.GetFire()
            };
        }
    }
}