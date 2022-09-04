using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Time Reference")]
    public class TimeFloatReference : FloatReference
    {
        public override float Value { get => UnityEngine.Time.time; set => base.Value = value; }
    }

    public abstract class FunctionValueReference : FloatReference
    {
        public FloatBusReference X;
        public override float Value { get => Function(X.Value); set => base.Value = value; }

        public abstract float Function(float x);
    }
}