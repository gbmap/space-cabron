using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCabron
{
    public class InjectBrainToActor : MonoBehaviour
    {
        public ScriptableBrain Brain;

        void Awake()
        {
            IBrainHolder[] brains = GetComponentsInChildren<IBrainHolder>();
            System.Array.ForEach(brains, b => b.Brain = Brain);
        }
    }
}