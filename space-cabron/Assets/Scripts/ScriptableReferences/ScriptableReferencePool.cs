using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Utils;

namespace Gmap.ScriptableReferences
{
    [System.Serializable]
    public class ScriptableReferenceItem<T> 
    {
        public ValueReference<T> Value;
        public int Weight;
    }

    public class ScriptableReferencePool<T> : ScriptableObject
    {
        public List<ScriptableReferenceItem<T>> Items = new List<ScriptableReferenceItem<T>>();
        
        ShuffleBag<ValueReference<T>> _shuffleBag;
        private ShuffleBag<ValueReference<T>> ShuffleBag
        {
            get 
            { 
                if (_shuffleBag == null)
                {
                    _shuffleBag = new ShuffleBag<ValueReference<T>>();
                    foreach (var item in Items)
                        _shuffleBag.Add(item.Value, item.Weight);
                }
                return _shuffleBag; 
            }
        }

        void Awake()
        {
        }

        public ValueReference<T> GetNext()
        {
            return ShuffleBag.Next();
        }
    }
}