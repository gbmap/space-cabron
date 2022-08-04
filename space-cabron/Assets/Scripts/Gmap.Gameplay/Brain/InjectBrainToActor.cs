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
            InjectBrainToActor<InputStateType>.Inject(gameObject, Brain);
            Destroy(this);
        }

        public static void Inject(GameObject target, IBrain<InputStateType> brain)
        {
            IBrainHolder<InputStateType>[] brains = target.GetComponentsInChildren<IBrainHolder<InputStateType>>();
            System.Array.ForEach(brains, b => b.Brain = brain);
        }
    }
}