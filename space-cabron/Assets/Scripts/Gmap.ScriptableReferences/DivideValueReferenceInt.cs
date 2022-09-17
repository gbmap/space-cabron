using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Divide (Int)")]
    public class DivideValueReferenceInt : IntReference
    {
        public IntBusReference A;
        public IntBusReference B;

        public override int Value { get => A.Value/B.Value; set => base.Value = value; }
    }
}