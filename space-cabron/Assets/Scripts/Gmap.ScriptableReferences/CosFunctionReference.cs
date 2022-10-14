
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Cos")]
    public class CosFunctionReference : FunctionValueReference
    {
        public override float Function(float x)
        {
            return Mathf.Cos(x);
        }
    }
}