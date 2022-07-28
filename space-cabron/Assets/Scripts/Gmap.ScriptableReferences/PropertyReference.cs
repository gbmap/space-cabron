using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ScriptableReferences
{
    [System.Serializable]
    public class PropertyReference<ValueType, WrappedType> where WrappedType : ValueReference<ValueType>
    {
        public WrappedType GetFrom;
        public WrappedType SendTo;

        [SerializeField] ValueType _value;
        public ValueType Value
        {
            get
            {
                if (GetFrom)
                    _value = GetFrom.Value;
                return _value;
            }
            set 
            {
                if (SendTo)
                    SendTo.Value = value;
                OnValueChanged?.Invoke(value);
                _value = value;
            }
        }

        public System.Action<ValueType> OnValueChanged;

        public void Update()
        {
            if (GetFrom)
                _value = GetFrom.Value;
            if (SendTo)
                SendTo.Value = _value;
        }
    }

    [System.Serializable]
    public class IntBusReference : PropertyReference<int, IntReference> {}

    [System.Serializable]
    public class FloatBusReference : PropertyReference<float, FloatReference> {}

    [System.Serializable]
    public class StringBusReference : PropertyReference<string, StringReference> {}
}