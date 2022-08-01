using Gmap.Gameplay;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    public abstract class ScriptableAIBrain<InputStateType> : ScriptableBrain<InputStateType>
    {
        public ScriptableMovementStrategy2D MovementStrategy;
        public ScriptableFiringStrategy FiringStrategy;

        public abstract override InputStateType GetInputState(InputStateArgs args);
    }
}