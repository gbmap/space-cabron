using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.Gameplay
{
    public class InjectBrainToActor<InputStateType> : MonoBehaviour
    {
        public ScriptableBrain<InputStateType> Brain;

        void Awake()
        {
            IBrainHolder<InputStateType>[] brains = GetComponentsInChildren<IBrainHolder<InputStateType>>();
            System.Array.ForEach(brains, b => b.Brain = Brain);
            Destroy(this);
        }
    }
}