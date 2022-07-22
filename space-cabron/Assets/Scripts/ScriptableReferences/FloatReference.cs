using UnityEngine;

namespace Gmap.ScriptableReferences
{
    public abstract class ValueReference<T> : ScriptableObject
    {
        [SerializeField]
        T value;

        public T Value
        {
            get { return value; }
            set { this.value = value;}
        }
    }

    [CreateAssetMenu(menuName="Gmap/Scriptable References/Float Reference")]
    public class FloatReference : ValueReference<float>{}
}