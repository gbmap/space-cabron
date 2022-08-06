using UnityEngine;

namespace Gmap.ScriptableReferences
{
    public abstract class ValueReference<T> : ScriptableObject
    {
        [SerializeField]
        T value;

        public virtual T Value
        {
            get { return value; }
            set { this.value = value;}
        }

        public static ValueReference<T> Create(T value)
        {
            var instance = ScriptableObject.CreateInstance<ValueReference<T>>();
            instance.Value = value;
            return instance;
        }
    }

    [CreateAssetMenu(menuName="Gmap/Scriptable References/Float Reference")]
    public class FloatReference : ValueReference<float>{}
}