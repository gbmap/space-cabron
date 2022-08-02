using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.Gameplay
{

    public abstract class ScriptableBrain<InputStateType> : ScriptableObject, IBrain<InputStateType>
    {
        public abstract InputStateType GetInputState(InputStateArgs args);
    }
}
