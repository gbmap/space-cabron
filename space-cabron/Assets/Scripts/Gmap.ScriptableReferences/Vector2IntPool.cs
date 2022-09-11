using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences {
    [CreateAssetMenu(menuName="Gmap/Scriptable References/Vector2Int Pool")]
    public class Vector2IntPool : ScriptableReferencePool<Vector2Int> {
        public override ScriptableReferencePool<Vector2Int> Clone() {
            var instance = ScriptableObject.CreateInstance<Vector2IntPool>();
            instance.SetItems(Items);
            return instance;
        }
    }
}