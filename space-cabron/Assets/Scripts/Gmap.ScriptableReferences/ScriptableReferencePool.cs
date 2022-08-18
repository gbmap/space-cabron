using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Utils;
using System.Linq;

namespace Gmap.ScriptableReferences
{
    [System.Serializable]
    public class ScriptableReferenceItem<T>
    {
        public T Value;
        public int Weight;
    }

    public interface ICloneable<T>
    {
        T Clone();
    }

    public class ScriptableReferencePool<T> : ScriptableObject, ICloneable<ScriptableReferencePool<T>>
    {
        public enum ERandomType
        {
            Random,
            ShuffleBag,
            ApplicationConstant,
            ShuffleBagNoRepeat,
            Sequential
        }
        public ERandomType RandomType = ERandomType.ShuffleBag;

        [SerializeField]
        protected List<ScriptableReferenceItem<T>> Items = new List<ScriptableReferenceItem<T>>();

        public IEnumerable<T> GetEnumerator()
        {
            return Items.Select(i => i.Value);
        }

        public int Length => Items.Count;

        private int currentSeed;

        public void ResetSeed()
        {
            currentSeed = Random.Range(0, int.MaxValue);
        }

        public void SetItems(IEnumerable<ScriptableReferenceItem<T>> items)
        {
            Items.Clear();
            Items.AddRange(items);
        }
        
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

        T[] noRepeatArray;
        private T[] NoRepeatArray
        {
            get 
            {
                if (noRepeatArray == null || noRepeatArray.Length == 0)
                {
                    try
                    {
                    noRepeatArray = ShuffleBag.NextNoRepeat(Items.Select(i=>i.Weight).Sum());
                    }
                    catch
                    {
                    noRepeatArray = ShuffleBag.Next(Items.Select(i=>i.Weight).Sum());
                    }
                }
                return noRepeatArray;
            }
        }
        int noRepeatCursor = 0;

        public T GetNext()
        {
            if (RandomType == ERandomType.Random)
                return Items[Random.Range(0, Items.Count)].Value;
            else if (RandomType == ERandomType.ApplicationConstant)
            {
                System.Random r = new System.Random();
                return Items[r.Next(0, Items.Count)].Value;
            }
            else if (RandomType == ERandomType.ShuffleBagNoRepeat)
            {
                return NoRepeatArray[(noRepeatCursor++) % NoRepeatArray.Length];
            }
            else
                return ShuffleBag.Next();
        }

        public T[] GetNext(int n)
        {
            if (RandomType == ERandomType.Random ||
                RandomType == ERandomType.ApplicationConstant)
                return Enumerable.Range(0, n).Select(i => GetNext()).ToArray();
            else
                return ShuffleBag.NextNoRepeat(n);
        }

        public virtual ScriptableReferencePool<T> Clone()
        {
            // Don't use generics here. Unity shits the bed and returns null when using it.
            var instance = ScriptableObject.CreateInstance(GetType()) as ScriptableReferencePool<T>;
            instance.Items = Items;
            return instance;
        }
    }
}