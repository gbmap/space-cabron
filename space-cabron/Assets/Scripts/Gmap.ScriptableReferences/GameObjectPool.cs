using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [CreateAssetMenu(menuName = "Gmap/Scriptable References/GameObject Scriptable Pool")]
    public class GameObjectPool : ScriptableReferencePool<GameObject>
    {
        public override ScriptableReferencePool<GameObject> Clone()
        {
            var instance = ScriptableObject.CreateInstance<GameObjectPool>();
            instance.SetItems(Items);
            return instance;
        }

    }
}