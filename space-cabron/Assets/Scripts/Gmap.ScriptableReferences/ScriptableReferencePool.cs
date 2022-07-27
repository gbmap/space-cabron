using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Utils;

namespace Gmap.ScriptableReferences
{
    [System.Serializable]
    public class ScriptableReferenceItem<T>
    {
        public T Value;
        public int Weight;
    }

    public class ScriptableReferencePool<T> : ScriptableObject
    {
        public enum ERandomType
        {
            Random,
            ShuffleBag
        }
        public ERandomType RandomType = ERandomType.ShuffleBag;

        [SerializeField]
        private List<ScriptableReferenceItem<T>> Items = new List<ScriptableReferenceItem<T>>();
        public int Length => Items.Count;
        
        ShuffleBag<T> _shuffleBag;
        private ShuffleBag<T> ShuffleBag
        {
            get 
            { 
                if (_shuffleBag == null)
                {
                    _shuffleBag = new ShuffleBag<T>();
                    foreach (var item in Items)
                        _shuffleBag.Add(item.Value, item.Weight);
                }
                return _shuffleBag; 
            }
        }

        public T GetNext()
        {
            if (RandomType == ERandomType.Random)
                return Items[Random.Range(0, Items.Count)].Value;
            else
                return ShuffleBag.Next();
        }
    }
}