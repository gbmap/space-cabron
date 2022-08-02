using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Random Int Reference")]
    public class RandomIntReference : IntReference
    {
        public int Min;
        public int Max;

        public override int Value 
        { 
            get => Random.Range(Min, Max); 
            set => base.Value = value; 
        }
    }
}