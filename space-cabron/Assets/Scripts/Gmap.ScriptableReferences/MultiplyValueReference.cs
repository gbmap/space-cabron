using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Multiply")]
    public class MultiplyValueReference : FloatReference
    {
        public FloatBusReference A;
        public FloatBusReference B;

        public override float Value { get => A.Value*B.Value; set => base.Value = value; }

    }
}