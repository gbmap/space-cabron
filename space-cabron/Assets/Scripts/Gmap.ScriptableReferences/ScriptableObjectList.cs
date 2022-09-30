using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    public class ScriptableObjectList<T> : ScriptableObject where T : ScriptableObject {
        [SerializeField] public List<T> List;

        public T this[int index] {
            get { return List[index]; }
            set { List[index] = (T)value; }
        }
    }
}