using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.Gameplay
{
    // Also known as skull.
    public interface IBrainHolder<InputStateType> {
        public IBrain<InputStateType> Brain {get; set;}
    }

    public interface IBrain<InputStateType>
    {
        public InputStateType GetInputState();
    }

    public static class Brain<InputStateType>
    {
        public static IBrain<InputStateType> Get(GameObject target)
        {
            IBrainHolder<InputStateType> brainHolder 
                = target.GetComponentInChildren<IBrainHolder<InputStateType>>();
            if (brainHolder != null)
                return brainHolder.Brain;
            return null;
        }
    }
}