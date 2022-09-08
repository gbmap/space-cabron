using UnityEngine;

namespace Gmap.Gameplay
{
    public class InjectBrainToActor<InputStateType> : MonoBehaviour
    {
        public bool InjectOnStart = true;
        public ScriptableBrain<InputStateType> Brain;

        void Start()
        {
            if (InjectOnStart)
            {
                Inject();
                Destroy(this);
            }
        }

        public void Inject()
        {
            InjectBrainToActor<InputStateType>.Inject(gameObject, Brain);
        }

        public static void Inject(GameObject target, IBrain<InputStateType> brain)
        {
            IBrainHolder<InputStateType>[] brains = target.GetComponentsInChildren<IBrainHolder<InputStateType>>();
            System.Array.ForEach(brains, b => b.Brain = brain);
        }
    }
}