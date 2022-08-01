using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    public abstract class ScriptableFiringStrategy : ScriptableObject
    {
        public abstract bool GetFire();
    }
}