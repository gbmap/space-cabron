using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Multiply (Int)")]
    public class MultiplyValueReferenceInt : IntReference
    {
        public IntBusReference A;
        public IntBusReference B;

        public override int Value { get => A.Value*B.Value; set => base.Value = value; }
    }
}