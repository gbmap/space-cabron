
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Sin")]
    public class SinFunctionReference : FunctionValueReference
    {
        public override float Function(float x)
        {
            return Mathf.Sin(x);
        }
    }
}