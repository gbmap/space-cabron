using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Subtract")]
    public class SubtractValueReference : FloatReference
    {
        public FloatBusReference A;
        public FloatBusReference B;
        public override float Value { get => A.Value-B.Value; set => base.Value = value; }
    }
}