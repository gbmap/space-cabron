using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Functions/Add")]
    public class AddValueReference : FloatReference
    {
        public FloatBusReference A;
        public FloatBusReference B;

        public override float Value { get => A.Value+B.Value; set => base.Value = value; }
    }
}